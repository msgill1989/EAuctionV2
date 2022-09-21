using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using SellerService.Models;
using SellerService.RepositoryLayer;
using SellerService.RepositoryLayer.Interfaces;

namespace SellerService.EventBusConsumer
{
    public class BidDateConsumer : IConsumer<GetBidDateRequestEvent>
    {
        private readonly IMapper _mapper;
        private readonly ISellerRepository _repository;
        private readonly ILogger<BidDateConsumer> _logger;

        public BidDateConsumer(IMapper mapper, ISellerRepository repository, ILogger<BidDateConsumer> logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetBidDateRequestEvent> context)
        {
            var getBidDateObj = _mapper.Map<BidDateRequest>(context.Message);

            var bidDateRes = await _repository.GetBidDateByProductId(getBidDateObj.ProductId);
        }
    }
}
