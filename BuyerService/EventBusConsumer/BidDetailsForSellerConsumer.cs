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
            try
            {
                var bidDetails = await _repository.GetAllBidsByProductId(context.Message.ProductId);
                if (bidDetails.BidDetails.Count == 0)
                {
                    _logger.LogError("There is no bid present for product with product Id: {0}", context.Message.ProductId);
                    throw new InvalidOperationException("No bid found");
                }
                var bidDetailsResEvent = _mapper.Map<GetBidDetailsResponseEvent>(bidDetails);
                await context.RespondAsync(bidDetailsResEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Some error happened in BidDetailsForSellerConsumer");
                throw;
            }

        }
    }
}
