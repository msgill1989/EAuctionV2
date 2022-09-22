using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Events
{
    public class GetBidDetailsResponseEvent : IntegrationBaseEvent
    {
        public List<Bids>? BidDetails { get; set; }
    }

    public class Bids
    {
        public string? BidId { get; set; }
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
