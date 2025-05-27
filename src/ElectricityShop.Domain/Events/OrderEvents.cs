using System;
using MediatR;

namespace ElectricityShop.Domain.Events
{
    /// <summary>
    /// Event fired when an order is created
    /// </summary>
    public class OrderCreatedEvent : INotification
    {
        /// <summary>
        /// ID of the created order
        /// </summary>
        public Guid OrderId { get; set; }
        
        /// <summary>
        /// Order number for customer reference
        /// </summary>
        public string OrderNumber { get; set; }
        
        /// <summary>
        /// ID of the customer who placed the order
        /// </summary>
        public Guid CustomerId { get; set; }
    }
    
    /// <summary>
    /// Event fired when order payment is processed
    /// </summary>
    public class OrderPaymentProcessedEvent : INotification
    {
        /// <summary>
        /// ID of the order
        /// </summary>
        public Guid OrderId { get; set; }
        
        /// <summary>
        /// Order number for customer reference
        /// </summary>
        public string OrderNumber { get; set; }
        
        /// <summary>
        /// Whether payment was successful
        /// </summary>
        public bool PaymentSuccess { get; set; }
        
        /// <summary>
        /// Transaction ID from payment provider
        /// </summary>
        public string TransactionId { get; set; }
        
        /// <summary>
        /// Error message if payment failed
        /// </summary>
        public string ErrorMessage { get; set; }
    }
    
    /// <summary>
    /// Event fired when an order fails
    /// </summary>
    public class OrderFailedEvent : INotification
    {
        /// <summary>
        /// ID of the order
        /// </summary>
        public Guid OrderId { get; set; }
        
        /// <summary>
        /// Order number for customer reference
        /// </summary>
        public string OrderNumber { get; set; }
        
        /// <summary>
        /// Reason for failure
        /// </summary>
        public string Reason { get; set; }
    }
    
    /// <summary>
    /// Event fired when an order is fulfilled
    /// </summary>
    public class OrderFulfilledEvent : INotification
    {
        /// <summary>
        /// ID of the order
        /// </summary>
        public Guid OrderId { get; set; }
        
        /// <summary>
        /// Order number for customer reference
        /// </summary>
        public string OrderNumber { get; set; }
    }
}