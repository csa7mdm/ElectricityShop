using System;

namespace ElectricityShop.Application.Features.Orders.Models
{
    /// <summary>
    /// Result of order creation operation
    /// </summary>
    public class OrderCreationResult
    {
        /// <summary>
        /// ID of the created order
        /// </summary>
        public Guid OrderId { get; set; }
        
        /// <summary>
        /// Order number (for customer reference)
        /// </summary>
        public string OrderNumber { get; set; }
        
        /// <summary>
        /// Current order status
        /// </summary>
        public OrderStatus Status { get; set; }
        
        /// <summary>
        /// Tracking ID for background processing
        /// </summary>
        public string TrackingId { get; set; }
        
        /// <summary>
        /// Estimated time for order processing completion
        /// </summary>
        public DateTime? EstimatedCompletion { get; set; }
    }
    
    /// <summary>
    /// Status of an order in the system
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Order has been created but processing has not started
        /// </summary>
        Created,
        
        /// <summary>
        /// Payment is in progress
        /// </summary>
        PaymentPending,
        
        /// <summary>
        /// Payment has been processed successfully
        /// </summary>
        PaymentProcessed,
        
        /// <summary>
        /// Order has been fulfilled
        /// </summary>
        Fulfilled,
        
        /// <summary>
        /// Order has been cancelled
        /// </summary>
        Cancelled,
        
        /// <summary>
        /// Order processing failed
        /// </summary>
        Failed
    }
}