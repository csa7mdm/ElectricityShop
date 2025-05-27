using System;

namespace ElectricityShop.Domain.Events.Orders
{
    /// <summary>
    /// Event that is triggered when an order is cancelled
    /// </summary>
    public class OrderCancelledEvent : DomainEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderCancelledEvent"/> class
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <param name="customerId">The customer ID</param>
        /// <param name="reason">The cancellation reason</param>
        /// <param name="cancelledById">The ID of the user who cancelled the order</param>
        public OrderCancelledEvent(
            Guid orderId,
            Guid customerId,
            string reason,
            Guid? cancelledById = null)
        {
            OrderId = orderId;
            CustomerId = customerId;
            Reason = reason;
            CancelledById = cancelledById;
            CancellationDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the order ID
        /// </summary>
        public Guid OrderId { get; }
        
        /// <summary>
        /// Gets the customer ID
        /// </summary>
        public Guid CustomerId { get; }
        
        /// <summary>
        /// Gets the cancellation reason
        /// </summary>
        public string Reason { get; }
        
        /// <summary>
        /// Gets the ID of the user who cancelled the order
        /// </summary>
        public Guid? CancelledById { get; }
        
        /// <summary>
        /// Gets the cancellation date
        /// </summary>
        public DateTime CancellationDate { get; }
    }
}