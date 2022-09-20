using SellerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SellerService.RepositoryLayer.Interfaces
{
    public interface ISellerRepository
    {
        Task AddProductAsync(ProductAndSeller productObj);
        Task<bool> DeleteProductAsync(string productId);
        Task<ShowBidsResponse> GetAllBidDetailsAsync(string productId);
    }
}
