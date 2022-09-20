using Microsoft.AspNetCore.Mvc.Infrastructure;
using MongoDB.Driver;

namespace SellerService.Data
{
    public class SellerContextSeed
    {
        public static void SeedData(IMongoCollection<ProductAndSeller> sellerCollection)
        {
            bool existProduct = sellerCollection.Find(x => true).Any();
            if (!existProduct)
            {
                sellerCollection.InsertManyAsync(GetPreconfiguredSellers());
            }
        }

        private static IEnumerable<ProductAndSeller> GetPreconfiguredSellers()
        {
            return new List<ProductAndSeller>()
            {
                new ProductAndSeller()
                {
                    Id = "6328cc7237d0f126ed3c366e",
                    ProductName = "Painting A",
                    ProductShortDescription = "Beautiful paiting",
                    ProductDetailedDescription = "Scenic beauty of Canada",
                    ProductCategory = "Painting",
                    ProductStartingPrice = 10.00,
                    BidEndDate = new DateTime(12/12/2022),
                    SellerFName = "Johny",
                    SellerLName = "Sanders",
                    Address = "123 Cityscape",
                    City = "Houston",
                    State = "TX",
                    Pin = "234567",
                    Phone = 9876543210,
                    Email = "test@gmail.com"

                },
                new ProductAndSeller()
                {
                    Id = "6328cc7237d0f126ed3c366f",
                    ProductName = "Sculptor B",
                    ProductShortDescription = "Beautiful Sculptor",
                    ProductDetailedDescription = "Best Sculptor of Canada",
                    ProductCategory = "Sculptor",
                    ProductStartingPrice = 11.00,
                    BidEndDate = new DateTime(12/12/2022),
                    SellerFName = "Mohan",
                    SellerLName = "Maken",
                    Address = "124 Cityscape",
                    City = "Chicago",
                    State = "WS",
                    Pin = "234567",
                    Phone = 9876543210,
                    Email = "test@gmail.com"
                },
                new ProductAndSeller()
                {
                    Id = "6328cc7237d0f126ed3c3670",
                    ProductName = "Ornament C",
                    ProductShortDescription = "Beautiful Ornament",
                    ProductDetailedDescription = "Best Ornament of Canada",
                    ProductCategory = "Ornament",
                    ProductStartingPrice = 13.00,
                    BidEndDate = new DateTime(12/12/2022),
                    SellerFName = "Andy",
                    SellerLName = "Murray",
                    Address = "125 Cityscape",
                    City = "Calgary",
                    State = "AB",
                    Pin = "234567",
                    Phone = 9876543210,
                    Email = "test@gmail.com"
                }
            };
        }
    }
}
