using BuyerService.Models;
using MongoDB.Driver;

namespace BuyerService.Data.Interfaces
{
    public interface IBuyerContext
    {
        IMongoCollection<BidAndBuyer> Buyers { get; }
    }
}
