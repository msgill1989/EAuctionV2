using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyerService.Models
{
    public class AllBidsForSellerResponse
    {
        private List<BidAndBuyer> bids = new List<BidAndBuyer>();
        public List<BidAndBuyer> BidDetails
        {
            set { bids = value; }
            get { return bids; }
        }
    }
}
