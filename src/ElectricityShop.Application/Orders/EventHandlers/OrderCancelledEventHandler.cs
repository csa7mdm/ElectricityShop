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
    /// Handler for the OrderCancelledEvent
    /// </summary>
    public class OrderCancelledEventHandler : IEventHandler<OrderCancelledEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IInventoryService _inventoryService;
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderCancelledEventHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderCancelledEventHandler"/> class
        /// </summary>
        /// <param name="notificationService">The notification service</param>
        /// <param name="inventoryService">The inventory service</param>
        /// <param name="orderService">The order service</param>
        /// <param name="logger">The logger</param>
        public OrderCancelledEventHandler(
            INotificationService notificationService,
            IInventoryService inventoryService,
            IOrderService orderService,
            ILogger<OrderCancelledEventHandler> logger)
        {
            _notificationService = notificationService;
            _inventoryService = inventoryService;
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Handles the OrderCancelledEvent
        /// </summary>
        /// <param name="event">The event to handle</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task HandleAsync(OrderCancelledEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Handling OrderCancelledEvent for order {OrderId}", @event.OrderId);

            try
            {
                // Get the order details to know what items to return to inventory
                var order = await _orderService.GetOrderAsync(@event.OrderId, cancellationToken);
                
                // Return inventory for each item in the order
                foreach (var item in order.Items)
                {
                    await _inventoryService.ReturnInventoryAsync(item.ProductId, item.Quantity, cancellationToken);
                }

                // Send cancellation notification to the customer
                await _notificationService.SendOrderCancelledAsync(
                    @event.CustomerId, 
                    @event.OrderId, 
                    @event.Reason, 
                    cancellationToken);
                
                _logger.LogInformation("Successfully processed OrderCancelledEvent for order {OrderId}", @event.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, 
                    "Error handling OrderCancelledEvent for order {OrderId}: {ErrorMessage}", 
                    @event.OrderId, ex.Message);
                
                // Rethrow the exception to trigger the retry mechanism in the event bus
                throw;
            }
        }
    }
}