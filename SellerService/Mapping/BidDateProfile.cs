using AutoMapper;
using EventBus.Messages.Events;
using SellerService.Models;

namespace SellerService.Mapping
{
    public class BidDateProfile :Profile
    {
        public BidDateProfile()
        {
            CreateMap<BidDateRequest, GetBidDateRequestEvent>().ReverseMap();
        }
    }
}
