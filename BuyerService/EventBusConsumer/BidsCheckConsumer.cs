using AutoMapper;
using BuyerService.RepositoryLayer.Interfaces;
using EventBus.Messages.Events;
using MassTransit;

namespace BuyerService.EventBusConsumer
{
    public class BidsCheckConsumer : IConsumer<BidsCheckRequestEvent>
    {
        private readonly ILogger<BidsCheckConsumer> _logger;
        private readonly IBuyerRepository _repository;
        private readonly IMapper _mapper;

        public BidsCheckConsumer(ILogger<BidsCheckConsumer> logger, IBuyerRepository repository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<BidsCheckRequestEvent> context)
        {
            try
            {
                var bidExistsRes = await _repository.IsBidPresent(context.Message.ProductId);
                var responseEventMsg = _mapper.Map<BidsCheckResponseEvent>(bidExistsRes);
                await context.RespondAsync(responseEventMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Seome internal error happened in BidsCheckConsumer class");
                throw;
            }
        }
    }
}
