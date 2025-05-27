using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ElectricityShop.Infrastructure.Events
{
    /// <summary>
    /// Implementation of the failed message logger
    /// </summary>
    public class FailedMessageLogger : IFailedMessageLogger
    {
        private readonly RabbitMqConnectionFactory _connectionFactory;
        private readonly RabbitMqSettings _settings;
        private readonly IApplicationDbContext _dbContext;
        private readonly ILogger<FailedMessageLogger> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedMessageLogger"/> class
        /// </summary>
        /// <param name="connectionFactory">The RabbitMQ connection factory</param>
        /// <param name="options">The RabbitMQ settings options</param>
        /// <param name="dbContext">The application database context</param>
        /// <param name="logger">The logger</param>
        public FailedMessageLogger(
            RabbitMqConnectionFactory connectionFactory,
            IOptions<RabbitMqSettings> options,
            IApplicationDbContext dbContext,
            ILogger<FailedMessageLogger> logger)
        {
            _connectionFactory = connectionFactory;
            _settings = options.Value;
            _dbContext = dbContext;
            _logger = logger;
        }

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
        public async Task LogFailedMessageAsync(
            string messageId,
            string eventType,
            string exchange,
            string routingKey,
            string message,
            string error,
            DateTime failedAt,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Create a new failed message entity
                var failedMessage = new Domain.Entities.FailedMessage
                {
                    Id = Guid.NewGuid(),
                    MessageId = messageId,
                    EventType = eventType,
                    Exchange = exchange,
                    RoutingKey = routingKey,
                    Message = message,
                    Error = error,
                    FailedAt = failedAt,
                    Processed = false
                };
                
                // Add the failed message to the database
                _dbContext.FailedMessages.Add(failedMessage);
                await _dbContext.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation(
                    "Logged failed message {MessageId} of type {EventType}",
                    messageId, eventType);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error logging failed message {MessageId} of type {EventType}: {ErrorMessage}",
                    messageId, eventType, ex.Message);
                
                throw;
            }
        }

        /// <summary>
        /// Gets a failed message by ID
        /// </summary>
        /// <param name="messageId">The message ID</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The failed message</returns>
        public async Task<FailedMessageDto> GetFailedMessageAsync(
            string messageId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Get the failed message from the database
                var failedMessage = await _dbContext.FailedMessages
                    .FirstOrDefaultAsync(m => m.MessageId == messageId, cancellationToken);
                
                if (failedMessage == null)
                {
                    return null;
                }
                
                return new FailedMessageDto
                {
                    MessageId = failedMessage.MessageId,
                    EventType = failedMessage.EventType,
                    Exchange = failedMessage.Exchange,
                    RoutingKey = failedMessage.RoutingKey,
                    Message = failedMessage.Message,
                    Error = failedMessage.Error,
                    FailedAt = failedMessage.FailedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error getting failed message {MessageId}: {ErrorMessage}",
                    messageId, ex.Message);
                
                throw;
            }
        }

        /// <summary>
        /// Requeues a failed message
        /// </summary>
        /// <param name="messageId">The message ID</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>True if the message was requeued; otherwise, false</returns>
        public async Task<bool> RequeueFailedMessageAsync(
            string messageId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Get the failed message from the database
                var failedMessage = await _dbContext.FailedMessages
                    .FirstOrDefaultAsync(m => m.MessageId == messageId, cancellationToken);
                
                if (failedMessage == null)
                {
                    _logger.LogWarning("Failed message {MessageId} not found", messageId);
                    return false;
                }
                
                // Republish the message to the original exchange
                using var channel = _connectionFactory.CreateChannel();
                
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent
                properties.ContentType = "application/json";
                properties.MessageId = failedMessage.MessageId;
                properties.Type = failedMessage.EventType;
                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                
                var body = Encoding.UTF8.GetBytes(failedMessage.Message);
                
                channel.BasicPublish(
                    exchange: failedMessage.Exchange,
                    routingKey: failedMessage.RoutingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
                
                // Mark the failed message as processed
                failedMessage.Processed = true;
                failedMessage.ProcessedAt = DateTime.UtcNow;
                
                await _dbContext.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation(
                    "Requeued failed message {MessageId} of type {EventType}",
                    messageId, failedMessage.EventType);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error requeuing failed message {MessageId}: {ErrorMessage}",
                    messageId, ex.Message);
                
                return false;
            }
        }
    }
}