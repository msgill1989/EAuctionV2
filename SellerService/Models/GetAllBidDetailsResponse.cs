using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SellerService.Models
{
    public class GetAllBidDetailsResponse
    {
        private List<BidDetails> bids = new();

        public List<BidDetails> BidDetails
        {
            set { bids = value; }
            get { return bids; }
        }
    }
}
