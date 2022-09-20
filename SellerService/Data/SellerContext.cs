using MongoDB.Driver;
using SellerService.Data.Interfaces;

namespace SellerService.Data
{
    public class SellerContext : ISellerContext
    {
        public SellerContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("EAuctionDatabase:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("EAuctionDatabase:DatabaseName"));

            Sellers = database.GetCollection<ProductAndSeller>(configuration.GetValue<string>("EAuctionDatabase:SellerCollectionName"));

            SellerContextSeed.SeedData(Sellers);
        }
        public IMongoCollection<ProductAndSeller> Sellers { get; }
    }
}
