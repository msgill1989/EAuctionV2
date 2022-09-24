using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Events
{
    public class BidsCheckResponseEvent : IntegrationBaseEvent
    {
        public string? ProductId { get; set; }

        public bool BidExists { get; set; }
    }
}
