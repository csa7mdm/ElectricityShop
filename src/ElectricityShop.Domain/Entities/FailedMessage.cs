using System;

namespace ElectricityShop.Domain.Entities
{
    /// <summary>
    /// Represents a failed message
    /// </summary>
    public class FailedMessage : BaseEntity
    {
        /// <summary>
        /// Gets or sets the message ID
        /// </summary>
        public string MessageId { get; set; }
        
        /// <summary>
        /// Gets or sets the event type
        /// </summary>
        public string EventType { get; set; }
        
        /// <summary>
        /// Gets or sets the exchange
        /// </summary>
        public string Exchange { get; set; }
        
        /// <summary>
        /// Gets or sets the routing key
        /// </summary>
        public string RoutingKey { get; set; }
        
        /// <summary>
        /// Gets or sets the message content
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string Error { get; set; }
        
        /// <summary>
        /// Gets or sets the time when the message failed
        /// </summary>
        public DateTime FailedAt { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the message has been processed
        /// </summary>
        public bool Processed { get; set; }
        
        /// <summary>
        /// Gets or sets the time when the message was processed
        /// </summary>
        public DateTime? ProcessedAt { get; set; }
    }
}