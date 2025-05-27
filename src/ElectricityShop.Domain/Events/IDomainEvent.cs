using System;

namespace ElectricityShop.Domain.Events
{
    /// <summary>
    /// Marker interface for domain events
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>
        /// Gets the unique identifier for this event
        /// </summary>
        Guid EventId { get; }
        
        /// <summary>
        /// Gets the timestamp when this event occurred
        /// </summary>
        DateTime OccurredOn { get; }
        
        /// <summary>
        /// Gets the event type name
        /// </summary>
        string EventType { get; }
        
        /// <summary>
        /// Gets the event version
        /// </summary>
        string Version { get; }
    }
}