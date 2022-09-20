using BuyerService.Data.Interfaces;
using BuyerService.Models;
using MongoDB.Driver;

namespace BuyerService.Data
{
    public class BuyerContext : IBuyerContext
    {
        public BuyerContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("EAuctionDatabase:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("EAuctionDatabase:DatabaseName"));

            Buyers = database.GetCollection<BidAndBuyer>(configuration.GetValue<string>("EAuctionDatabase:BuyerCollectionName"));

            BuyerContextSeed.SeedData(Buyers);
        }
        public IMongoCollection<BidAndBuyer> Buyers { get; }
    }
}
