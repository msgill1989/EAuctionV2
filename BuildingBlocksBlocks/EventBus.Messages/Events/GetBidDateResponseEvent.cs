using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Events
{
    public class GetBidDateResponseEvent : IntegrationBaseEvent
    {
        public string? ProductId { get; set; }
        public DateTime? BidEndDate { get; set; }
    }
}
