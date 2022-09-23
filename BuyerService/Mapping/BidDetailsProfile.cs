using AutoMapper;
using BuyerService.Models;
using EventBus.Messages.Events;

namespace BuyerService.Mapping
{
    public class BidDetailsProfile : Profile
    {
        public BidDetailsProfile()
        {
            CreateMap<AllBidsForSellerResponse, GetBidDetailsResponseEvent>().ReverseMap();
            CreateMap<BidAndBuyer, Bids>().ReverseMap();
        }
    }
}
