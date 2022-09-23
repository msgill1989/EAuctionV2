using AutoMapper;
using EventBus.Messages.Events;
using SellerService.Models;

namespace SellerService.Mapping
{
    public class BidDetailsProfile : Profile
    {
        public BidDetailsProfile()
        {
            CreateMap<GetBidDetailsResponseEvent, GetAllBidDetailsResponse>().ReverseMap();
            CreateMap<Bids, BidDetails>().ReverseMap(); 
        }
    }
}
