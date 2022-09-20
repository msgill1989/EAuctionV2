using BuyerService.Models;
using MongoDB.Driver;

namespace BuyerService.Data
{
    public class BuyerContextSeed
    {
        public static void SeedData(IMongoCollection<BidAndBuyer> buyerCollection)
        {
            bool existBid = buyerCollection.Find(x => true).Any();

            if (!existBid)
            {
                buyerCollection.InsertManyAsync(GetPreConfiguredBuyers());
            }
        }

        private static IEnumerable<BidAndBuyer> GetPreConfiguredBuyers()
        {
            return new List<BidAndBuyer>()
            {
                new BidAndBuyer()
                {
                     Id = "",
                     BuyerFName = "JohnyBuyer",
                     BuyerLName = "SandersBuyer",
                     Address = "123 Cityscape",
                     City = "Houston",
                     State = "TX",
                     Pin = "234567",
                     Phone = 9876543210,
                     Email = "test@gmail.com",
                     ProductId = "6328cc7237d0f126ed3c366e",
                     BidAmount = 12.00
                },
                new BidAndBuyer()
                {
                     Id = "",
                     BuyerFName = "MohanBuyer",
                     BuyerLName = "MakenBuyer",
                     Address = "124 Cityscape",
                     City = "Chicago",
                     State = "Washington",
                     Pin = "234567",
                     Phone = 9876543210,
                     Email = "test@gmail.com",
                     ProductId = "6328cc7237d0f126ed3c366f",
                     BidAmount = 13.00
                },
                new BidAndBuyer()
                {
                     Id = "",
                     BuyerFName = "AndyBuyer",
                     BuyerLName = "MurrayBuyer",
                     Address = "125 Cityscape",
                     City = "Calgary",
                     State = "AB",
                     Pin = "234567",
                     Phone = 9876543210,
                     Email = "test@gmail.com",
                     ProductId = "6328cc7237d0f126ed3c3670",
                     BidAmount =11.00

                }
            };
        }
    }
}
