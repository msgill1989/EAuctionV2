using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Common
{
    public static class EventBusConstants
    {
        public const string GetBidEndDateforInsertBidQueue = "getbidenddateforinsertbid-queue";
        public const string GetBidEndDateforUpdBidAmountQueue = "getbidenddateforupdbidamount-queue";
        public const string GetBidDetailsQueue = "getbiddetails-queue";
        public const string BidsCheckQueue = "bidscheck-queue";
    }
}
