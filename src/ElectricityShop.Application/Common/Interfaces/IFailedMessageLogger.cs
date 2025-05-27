using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for logging failed messages
    /// </summary>
    public interface IFailedMessageLogger
    {
        /// <summary>
        /// Logs a failed message
        /// </summary>
        /// <param name="messageId">The message ID</param>
        /// <param name="eventType">The event type</param>
        /// <param name="exchange">The exchange</param>
        /// <param name="routingKey">The routing key</param>
        /// <param name="message">The message content</param>
        /// <param name="error">The error message</param>
        /// <param name="failedAt">The time when the message failed</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task LogFailedMessageAsync(
            string messageId,
            string eventType,
            string exchange,
            string routingKey,
            string message,
            string error,
            DateTime failedAt,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a failed message by ID
        /// </summary>
        /// <param name="messageId">The message ID</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The failed message</returns>
        Task<FailedMessageDto> GetFailedMessageAsync(
            string messageId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Requeues a failed message
        /// </summary>
        /// <param name="messageId">The message ID</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>True if the message was requeued; otherwise, false</returns>
        Task<bool> RequeueFailedMessageAsync(
            string messageId,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Data transfer object for failed messages
    /// </summary>
    public class FailedMessageDto
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
    }
}