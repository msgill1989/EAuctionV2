using BuyerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyerService.RepositoryLayer.Interfaces
{
    public interface IBuyerRepository
    {
        Task AddBid(BidAndBuyer bidDetails);
        Task<bool> UpdateBid(string productId, string buyerEmailId, double bidAmount);

        Task<AllBidsForSellerResponse> GetAllBidsByProductId(string? productId);
    }
}
