using System;

namespace SellerService.Models
{
    public class BidDateResponse
    {
        public string? ProductId { get; set; }

        public DateTime? BidDate { get; set; }
    }
}
