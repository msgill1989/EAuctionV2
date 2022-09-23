using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyerService.Data.Interfaces;
using BuyerService.Models;
using BuyerService.RepositoryLayer.Interfaces;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BuyerService.RepositoryLayer
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly ILogger<BuyerRepository> _logger;
        private readonly IBuyerContext _context;
        //private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<GetBidDateRequestEvent> _client;
        public BuyerRepository(ILogger<BuyerRepository> logger, IBuyerContext context, IRequestClient<GetBidDateRequestEvent> client) //, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _context = context;
            //_publishEndpoint = publishEndpoint;
            _client = client;
        }
        public async Task AddBid(BidAndBuyer bidDetails)
        {
            try
            {
                //If bid is placed after bid end date. Throw an exception
                GetBidDateRequestEvent eventMessage = new GetBidDateRequestEvent() { ProductId = bidDetails.ProductId };
                //var a = await _publishEndpoint.Publish(eventMessage);
                var result = await _client.GetResponse<GetBidDateResponseEvent>(eventMessage);
                if (DateTime.Now > result.Message.BidEndDate)
                    throw new KeyNotFoundException("The Bid cannot be placed after the bid end date.");


                //Check if the same user has already placed a bid
                BidAndBuyer existingBid = await GetBidDetails(bidDetails.ProductId, bidDetails.Email);
                if (existingBid != null)
                    throw new KeyNotFoundException("This buyer has already placed bid for this product.");


                await _context.Buyers.InsertOneAsync(bidDetails);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Some error happened while adding the bid details to DB");
                throw;
            }
        }
        public async Task<bool> UpdateBid(string productId, string buyerEmailId, double bidAmount)
        {
            try
            {
                //If bid Amount is updated after the bid end date throw exception. 
                GetBidDateRequestEvent eventMessage = new GetBidDateRequestEvent() { ProductId = productId };
                var result = await _client.GetResponse<GetBidDateResponseEvent>(eventMessage);
                if (DateTime.Now > result.Message.BidEndDate)
                    throw new KeyNotFoundException("The Bid amount cannot be updated after Bid end date.");

                var earlierBidDetails = await _context.Buyers.Find(x => x.ProductId == productId && x.Email == buyerEmailId).FirstOrDefaultAsync();
                earlierBidDetails.BidAmount = bidAmount;
                var updateResult = await _context.Buyers.ReplaceOneAsync(y => y.ProductId == productId && y.Email == buyerEmailId, earlierBidDetails);

                return updateResult.IsAcknowledged
                    && updateResult.ModifiedCount > 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<AllBidsForSellerResponse> GetAllBidsByProductId(string? productId)
        {
            try
            {
                AllBidsForSellerResponse result = new AllBidsForSellerResponse();
                var response = await _context.Buyers.Find(x => x.ProductId == productId).ToListAsync();
                foreach (var bid in response)
                {
                    result.BidDetails.Add(bid);
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<BidAndBuyer> GetBidDetails(string? productId, string? bidderEmailId = null)
        {
            try
            {
                BidAndBuyer bidDetails;
                if (bidderEmailId == null)
                {
                    bidDetails = await _context.Buyers.Find(x => x.ProductId == productId).FirstOrDefaultAsync();
                }
                else
                {
                    bidDetails = await _context.Buyers.Find(x => x.Email == bidderEmailId && x.ProductId == productId).FirstOrDefaultAsync();
                }
                return bidDetails;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
