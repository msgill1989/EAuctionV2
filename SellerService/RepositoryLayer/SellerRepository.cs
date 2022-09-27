using SellerService.RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SellerService.Models;
using MongoDB.Driver;
using SellerService.Data.Interfaces;
using MassTransit;
using EventBus.Messages.Events;
using AutoMapper;

namespace SellerService.RepositoryLayer
{
    public class SellerRepository : ISellerRepository
    {
        private readonly ILogger<SellerRepository> _logger;
        private readonly ISellerContext _context;
        private readonly IRequestClient<GetBidDetailsRequestEvent> _client;
        private readonly IRequestClient<BidsCheckRequestEvent> _bidsCheckClient;
        private readonly IMapper _mapper;

        public SellerRepository(ILogger<SellerRepository> logger, ISellerContext context, IRequestClient<GetBidDetailsRequestEvent> client, IRequestClient<BidsCheckRequestEvent> bidsCheckClient, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _bidsCheckClient = bidsCheckClient ?? throw new ArgumentNullException(nameof(bidsCheckClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task AddProductAsync(ProductAndSeller productObj)
        {
            try
            {
                await _context.Sellers.InsertOneAsync(productObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error happened while adding the product to Database");
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(string productId)
        {
            try
            {
                //Get the product Details to check if bid date is valid
                var productDetails = await GetProductByProductIdAsync(productId);

                ////Check the Bid end date
                if (productDetails.BidEndDate < DateTime.UtcNow)
                    throw new KeyNotFoundException("Product cannot be deleted after the BidEnd date.");

                //If Any bid is already placed Dont delete the product
                BidsCheckRequestEvent eventMessage = new BidsCheckRequestEvent() { ProductId = productId };
                var response = await _bidsCheckClient.GetResponse<BidsCheckResponseEvent>(eventMessage);
                if (response.Message.BidExists)
                {
                    var errorMsg = string.Format("This product with productId: {0} cannot be deleted because bids are present.", productId);
                    _logger.LogError(errorMsg);
                    throw new KeyNotFoundException(errorMsg);
                }


                FilterDefinition<ProductAndSeller> filter = Builders<ProductAndSeller>.Filter.Eq(x => x.Id, productId);
                DeleteResult deleteResult = await _context
                                                        .Sellers
                                                        .DeleteOneAsync(filter);
                return deleteResult.IsAcknowledged
                    && deleteResult.DeletedCount > 0;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "The product with product id {0} cannot be deleted because bid end date has already passed.", productId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error happened while deleting the product with Id: {0}", productId);
                throw;
            }
        }

        public async Task<ShowBidsResponse> GetAllBidDetailsAsync(string productId)
        {
            try
            {
                ShowBidsResponse getAllBidsResponse = new ShowBidsResponse();
                var productDetails = await GetProductByProductIdAsync(productId);

                getAllBidsResponse.ProductId = productDetails.Id;
                getAllBidsResponse.ProductName = productDetails.ProductName;
                getAllBidsResponse.ShortDescription = productDetails.ProductShortDescription;
                getAllBidsResponse.DetailedDescription = productDetails.ProductDetailedDescription;
                getAllBidsResponse.StartingPrice = productDetails.ProductStartingPrice;
                getAllBidsResponse.Category = productDetails.ProductCategory;
                getAllBidsResponse.BidEndDate = productDetails.BidEndDate;

                //Get all the bids using RabbitMq
                GetBidDetailsRequestEvent eventMessage = new GetBidDetailsRequestEvent() { ProductId = productId };
                var response = await _client.GetResponse<GetBidDetailsResponseEvent>(eventMessage);
                if (response.Message.BidDetails.Count > 0)
                {
                    var res = _mapper.Map<GetAllBidDetailsResponse>(response.Message);
                    foreach (var bid in res.BidDetails)
                    {
                        getAllBidsResponse.Bids.Add(bid);
                    }
                }

                return getAllBidsResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Some error happened while fetching the bid details for product with id {0}", productId);
                throw;
            }
        }

        public async Task<BidDateResponse> GetBidDateByProductId(string productId)
        {
            BidDateResponse bidDateResponse = new BidDateResponse();    
            var productDetails = await GetProductByProductIdAsync(productId);
            if (productDetails != null)
            {
                bidDateResponse.ProductId = productDetails.Id;
                bidDateResponse.BidEndDate = productDetails.BidEndDate;
            } 
            else
            {
                bidDateResponse.ProductId = productId;
                bidDateResponse.BidEndDate = null;
            }
            return bidDateResponse;
        }

        private async Task<ProductAndSeller> GetProductByProductIdAsync(string productId)
        {
            try
            {
                return await _context
                                .Sellers
                                .Find(x => x.Id == productId)
                                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error happened while getting the product with product ID {0}", productId);
                throw;
            }
        }
    }
}
