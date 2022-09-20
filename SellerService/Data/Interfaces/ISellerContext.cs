using MongoDB.Driver;

namespace SellerService.Data.Interfaces
{
    public interface ISellerContext
    {
        IMongoCollection<ProductAndSeller> Sellers { get; }
    }
}
