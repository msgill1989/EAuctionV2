using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SellerService.Models
{
    public class BidDetails
    {
        public string? Id { get; set; }
        public string? BuyerFName { get; set; }
        public string? BuyerLName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Pin { get; set; }
        public long Phone { get; set; }
        public string? Email { get; set; }
        public string? ProductId { get; set; }
        public double BidAmount { get; set; }
    }
}
