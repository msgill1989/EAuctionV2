using AutoMapper;
using Confluent.Kafka;
using EventBus.Messages.Events;
using MassTransit;
using SellerService.RepositoryLayer.Interfaces;

namespace SellerService.EventBusConsumer
{
    public class BidDateForUpdBidAmountConsumer : IConsumer<GetBidDateRequestEvent>
    {
        private readonly IMapper _mapper;
        private readonly ISellerRepository _repository;
        private readonly ILogger<BidDateForUpdBidAmountConsumer> _logger;

        public BidDateForUpdBidAmountConsumer(IMapper mapper, ISellerRepository repository, ILogger<BidDateForUpdBidAmountConsumer> logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetBidDateRequestEvent> context)
        {
            var bidDateRes = await _repository.GetBidDateByProductId(context.Message.ProductId);
            if (bidDateRes == null)
            {
                _logger.LogError("The product with product Id {0} is not found", context.Message.ProductId);
                throw new InvalidOperationException("Product not found");
            }

            await context.RespondAsync(new GetBidDateResponseEvent()
            {
                ProductId = bidDateRes.ProductId,
                BidEndDate = bidDateRes.BidEndDate
            });
        }
    }
}
