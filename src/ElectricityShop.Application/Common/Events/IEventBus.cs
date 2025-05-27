using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Domain.Events;

namespace ElectricityShop.Application.Common.Events
{
    /// <summary>
    /// Defines the event bus interface for publishing domain events
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publishes a domain event to the event bus
        /// </summary>
        /// <typeparam name="TEvent">The type of event</typeparam>
        /// <param name="event">The event to publish</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
            where TEvent : IDomainEvent;
    }
}