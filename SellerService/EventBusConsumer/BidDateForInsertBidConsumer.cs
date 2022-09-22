using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using SellerService.Models;
using SellerService.RepositoryLayer;
using SellerService.RepositoryLayer.Interfaces;

namespace SellerService.EventBusConsumer
{
    public class BidDateForInsertBidConsumer : IConsumer<GetBidDateRequestEvent>
    {
        private readonly IMapper _mapper;
        private readonly ISellerRepository _repository;
        private readonly ILogger<BidDateForInsertBidConsumer> _logger;

        public BidDateForInsertBidConsumer(IMapper mapper, ISellerRepository repository, ILogger<BidDateForInsertBidConsumer> logger)
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

            await context.RespondAsync<GetBidDateResponseEvent>( new GetBidDateResponseEvent()
                { 
                ProductId = bidDateRes.ProductId,
                BidEndDate = bidDateRes.BidEndDate
            });
        }
    }
}
