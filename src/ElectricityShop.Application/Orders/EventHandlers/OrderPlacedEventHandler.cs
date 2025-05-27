using System;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Common.Events;
using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Domain.Events.Orders;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.Application.Orders.EventHandlers
{
    /// <summary>
    /// Handler for the OrderPlacedEvent
    /// </summary>
    public class OrderPlacedEventHandler : IEventHandler<OrderPlacedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<OrderPlacedEventHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderPlacedEventHandler"/> class
        /// </summary>
        /// <param name="notificationService">The notification service</param>
        /// <param name="inventoryService">The inventory service</param>
        /// <param name="logger">The logger</param>
        public OrderPlacedEventHandler(
            INotificationService notificationService,
            IInventoryService inventoryService,
            ILogger<OrderPlacedEventHandler> logger)
        {
            _notificationService = notificationService;
            _inventoryService = inventoryService;
            _logger = logger;
        }

        /// <summary>
        /// Handles the OrderPlacedEvent
        /// </summary>
        /// <param name="event">The event to handle</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task HandleAsync(OrderPlacedEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Handling OrderPlacedEvent for order {OrderId}", @event.OrderId);

            try
            {
                // Update inventory for each ordered item
                foreach (var item in @event.OrderItems)
                {
                    await _inventoryService.DeductInventoryAsync(item.ProductId, item.Quantity, cancellationToken);
                }

                // Send notification to customer
                await _notificationService.SendOrderConfirmationAsync(
                    @event.CustomerId, 
                    @event.OrderId, 
                    @event.TotalAmount, 
                    cancellationToken);
                
                _logger.LogInformation("Successfully processed OrderPlacedEvent for order {OrderId}", @event.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling OrderPlacedEvent for order {OrderId}: {ErrorMessage}", 
                    @event.OrderId, ex.Message);
                
                // Rethrow the exception to trigger the retry mechanism in the event bus
                throw;
            }
        }
    }
}