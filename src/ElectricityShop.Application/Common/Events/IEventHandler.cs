using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Domain.Events;

namespace ElectricityShop.Application.Common.Events
{
    /// <summary>
    /// Defines an event handler interface for handling domain events
    /// </summary>
    /// <typeparam name="TEvent">The type of domain event</typeparam>
    public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        /// <summary>
        /// Handles the event
        /// </summary>
        /// <param name="event">The event to handle</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
    }
}