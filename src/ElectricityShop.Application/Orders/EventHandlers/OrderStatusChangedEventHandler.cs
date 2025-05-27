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
    /// Handler for the OrderStatusChangedEvent
    /// </summary>
    public class OrderStatusChangedEventHandler : IEventHandler<OrderStatusChangedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<OrderStatusChangedEventHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderStatusChangedEventHandler"/> class
        /// </summary>
        /// <param name="notificationService">The notification service</param>
        /// <param name="logger">The logger</param>
        public OrderStatusChangedEventHandler(
            INotificationService notificationService,
            ILogger<OrderStatusChangedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Handles the OrderStatusChangedEvent
        /// </summary>
        /// <param name="event">The event to handle</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task HandleAsync(OrderStatusChangedEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Handling OrderStatusChangedEvent for order {OrderId} - Status changed from {PreviousStatus} to {NewStatus}", 
                @event.OrderId, @event.PreviousStatus, @event.NewStatus);

            try
            {
                // Send notification to customer about the status change
                await _notificationService.SendOrderStatusChangedAsync(
                    @event.CustomerId, 
                    @event.OrderId, 
                    @event.NewStatus, 
                    cancellationToken);
                
                _logger.LogInformation(
                    "Successfully processed OrderStatusChangedEvent for order {OrderId}", 
                    @event.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, 
                    "Error handling OrderStatusChangedEvent for order {OrderId}: {ErrorMessage}", 
                    @event.OrderId, ex.Message);
                
                // Rethrow the exception to trigger the retry mechanism in the event bus
                throw;
            }
        }
    }
}