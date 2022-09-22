using AutoMapper;
using BuyerService.RepositoryLayer.Interfaces;
using EventBus.Messages.Events;
using MassTransit;

namespace BuyerService.EventBusConsumer
{
    public class BidDetailsForSellerConsumer : IConsumer<GetBidDetailsRequestEvent>
    {
        private readonly ILogger<BidDetailsForSellerConsumer> _logger;
        private readonly IBuyerRepository _repository;
        private readonly IMapper _mapper;

        public BidDetailsForSellerConsumer(ILogger<BidDetailsForSellerConsumer> logger, IBuyerRepository repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<GetBidDetailsRequestEvent> context)
        {
            var bidDetails = _repository.GetAllBidsByProductId(context.Message.ProductId);
            if (bidDetails != null)
            {
                _logger.LogError("There is no bid present for product with product Id: {0}", context.Message.ProductId);
                throw new InvalidOperationException("No bid found");
            }
            await context.RespondAsync<GetBidDetailsResponseEvent>(new GetBidDetailsResponseEvent()
            {
                //Send the list of all Bids
            });
        }
    }
}
