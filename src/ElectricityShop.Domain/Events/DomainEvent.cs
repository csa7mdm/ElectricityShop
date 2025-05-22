using System;

namespace ElectricityShop.Domain.Events
{
    /// <summary>
    /// Base implementation of domain event
    /// </summary>
    public abstract class DomainEvent : IDomainEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEvent"/> class.
        /// </summary>
        protected DomainEvent()
        {
            EventId = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            EventType = GetType().Name;
            Version = "1.0"; // Default version
        }

        /// <summary>
        /// Gets the unique identifier for this event
        /// </summary>
        public Guid EventId { get; private set; }
        
        /// <summary>
        /// Gets the timestamp when this event occurred
        /// </summary>
        public DateTime OccurredOn { get; private set; }
        
        /// <summary>
        /// Gets the event type name
        /// </summary>
        public string EventType { get; private set; }
        
        /// <summary>
        /// Gets the event version
        /// </summary>
        public string Version { get; protected set; }
    }
}