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

namespace SellerService.RepositoryLayer
{
    public class SellerRepository : ISellerRepository
    {
        private readonly ILogger<SellerRepository> _logger;
        private readonly ISellerContext _context; 
        
        public SellerRepository(ILogger<SellerRepository> logger, ISellerContext context)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
                if (productDetails.BidEndDate < DateTime.Now)
                    throw new KeyNotFoundException("Product cannot be deleted after the BidEnd date.");

                // //If Any bid is already placed Dont delete the product



                FilterDefinition<ProductAndSeller> filter = Builders<ProductAndSeller>.Filter.Eq(x => x.Id, productId);
                DeleteResult deleteResult = await _context
                                                        .Sellers
                                                        .DeleteOneAsync(filter);
                return deleteResult.IsAcknowledged
                    && deleteResult.DeletedCount > 0;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError("The product with product id {0} cannot be deleted because bid end date has already passed.", productId);
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

                getAllBidsResponse.ProductName = productDetails.ProductName;
                getAllBidsResponse.ShortDescription = productDetails.ProductShortDescription;
                getAllBidsResponse.DetailedDescription = productDetails.ProductDetailedDescription;
                getAllBidsResponse.StartingPrice = productDetails.ProductStartingPrice;
                getAllBidsResponse.Category = productDetails.ProductCategory;
                getAllBidsResponse.BidEndDate = productDetails.BidEndDate;

                //Get all the bids using Kafka

                return getAllBidsResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Some error happened while fetching the bid details for product with id {0}", productId);
                throw;
            }
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
