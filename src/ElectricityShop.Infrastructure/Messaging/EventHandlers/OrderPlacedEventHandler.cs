using System.Threading.Tasks;
using ElectricityShop.Infrastructure.Messaging.Events;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.Infrastructure.Messaging.EventHandlers
{
    public class OrderPlacedEventHandler
    {
        private readonly ILogger<OrderPlacedEventHandler> _logger;

        public OrderPlacedEventHandler(ILogger<OrderPlacedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(OrderPlacedEvent @event)
        {
            _logger.LogInformation("Handling OrderPlaced event for order {OrderId} ({OrderNumber})", @event.OrderId, @event.OrderNumber);

            // Here we would implement business logic for handling a new order
            // For example:
            // 1. Send confirmation email to customer
            // 2. Notify warehouse for fulfillment
            // 3. Update inventory systems
            // 4. Generate invoice
            // etc.

            await Task.CompletedTask;
        }
    }
}