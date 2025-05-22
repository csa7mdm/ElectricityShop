using System;
using System.Collections.Generic;
using ElectricityShop.Domain.Events;

namespace ElectricityShop.Infrastructure.Events
{
    /// <summary>
    /// Registry for event types
    /// </summary>
    public class EventTypeRegistry
    {
        /// <summary>
        /// Gets the dictionary of event types
        /// </summary>
        public Dictionary<string, Type> EventTypes { get; } = new Dictionary<string, Type>();

        /// <summary>
        /// Registers an event type
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        public void RegisterEventType<TEvent>() where TEvent : IDomainEvent
        {
            var eventType = typeof(TEvent);
            var eventName = eventType.Name;
            
            if (!EventTypes.ContainsKey(eventName))
            {
                EventTypes.Add(eventName, eventType);
            }
        }

        /// <summary>
        /// Registers an event type
        /// </summary>
        /// <param name="eventType">The event type</param>
        public void RegisterEventType(Type eventType)
        {
            if (!typeof(IDomainEvent).IsAssignableFrom(eventType))
            {
                throw new ArgumentException($"Type {eventType.Name} does not implement IDomainEvent", nameof(eventType));
            }
            
            var eventName = eventType.Name;
            
            if (!EventTypes.ContainsKey(eventName))
            {
                EventTypes.Add(eventName, eventType);
            }
        }
    }
}