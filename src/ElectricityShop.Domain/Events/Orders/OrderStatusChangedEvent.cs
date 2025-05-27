using System;

namespace ElectricityShop.Domain.Events.Orders
{
    /// <summary>
    /// Event that is triggered when an order status changes
    /// </summary>
    public class OrderStatusChangedEvent : DomainEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderStatusChangedEvent"/> class
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <param name="customerId">The customer ID</param>
        /// <param name="previousStatus">The previous order status</param>
        /// <param name="newStatus">The new order status</param>
        /// <param name="changedById">The ID of the user who changed the status</param>
        /// <param name="notes">Optional notes about the status change</param>
        public OrderStatusChangedEvent(
            Guid orderId,
            Guid customerId,
            string previousStatus,
            string newStatus,
            Guid? changedById = null,
            string notes = null)
        {
            OrderId = orderId;
            CustomerId = customerId;
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
            ChangedById = changedById;
            Notes = notes;
            ChangedDate = DateTime.UtcNow;
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
        /// Gets the previous order status
        /// </summary>
        public string PreviousStatus { get; }
        
        /// <summary>
        /// Gets the new order status
        /// </summary>
        public string NewStatus { get; }
        
        /// <summary>
        /// Gets the ID of the user who changed the status
        /// </summary>
        public Guid? ChangedById { get; }
        
        /// <summary>
        /// Gets notes about the status change
        /// </summary>
        public string Notes { get; }
        
        /// <summary>
        /// Gets the date when the status changed
        /// </summary>
        public DateTime ChangedDate { get; }
    }
}