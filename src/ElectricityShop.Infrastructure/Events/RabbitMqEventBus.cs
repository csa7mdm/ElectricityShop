using ElectricityShop.Application.Common.Events;
using ElectricityShop.Domain.Events;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ElectricityShop.Infrastructure.Events
{
    /// <summary>
    /// RabbitMQ implementation of the event bus
    /// </summary>
    public class RabbitMqEventBus : IEventBus, IDisposable
    {
        private readonly RabbitMqConnectionFactory _connectionFactory;
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<RabbitMqEventBus> _logger;
        private IModel _channel;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqEventBus"/> class
        /// </summary>
        /// <param name="connectionFactory">The RabbitMQ connection factory</param>
        /// <param name="options">The RabbitMQ settings options</param>
        /// <param name="logger">The logger</param>
        public RabbitMqEventBus(
            RabbitMqConnectionFactory connectionFactory,
            IOptions<RabbitMqSettings> options,
            ILogger<RabbitMqEventBus> logger)
        {
            _connectionFactory = connectionFactory;
            _settings = options.Value;
            _logger = logger;

            Initialize();
        }

        /// <summary>
        /// Publishes a domain event to the event bus
        /// </summary>
        /// <typeparam name="TEvent">The type of event</typeparam>
        /// <param name="event">The event to publish</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            var eventName = @event.GetType().Name;
            var routingKey = eventName.ToLower();

            _logger.LogInformation("Publishing event {EventName} with id {EventId}", eventName, @event.EventId);

            // Ensure channel is available
            EnsureChannel();

            // Serialize event to JSON
            var message = JsonSerializer.Serialize(@event, @event.GetType());
            var body = Encoding.UTF8.GetBytes(message);

            // Set message properties
            var properties = _channel.CreateBasicProperties();
            properties.DeliveryMode = 2; // persistent
            properties.ContentType = "application/json";
            properties.MessageId = @event.EventId.ToString();
            properties.Type = eventName;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.Headers = new System.Collections.Generic.Dictionary<string, object>
            {
                ["x-event-version"] = @event.Version
            };

            // Publish the message
            _channel.BasicPublish(
                exchange: _settings.ExchangeName,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: properties,
                body: body);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Initializes the RabbitMQ exchange and dead letter exchange
        /// </summary>
        private void Initialize()
        {
            _channel = _connectionFactory.CreateChannel();

            // Declare the main exchange
            _channel.ExchangeDeclare(
                exchange: _settings.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            // Declare the dead letter exchange
            _channel.ExchangeDeclare(
                exchange: _settings.DeadLetterExchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            // Add return listener for unroutable messages
            _channel.BasicReturn += (sender, args) =>
            {
                _logger.LogWarning("Message returned: {ReplyText}, Exchange: {Exchange}, RoutingKey: {RoutingKey}",
                    args.ReplyText, args.Exchange, args.RoutingKey);
            };
        }

        /// <summary>
        /// Ensures that the channel is available, creating a new one if needed
        /// </summary>
        private void EnsureChannel()
        {
            if (_channel == null || _channel.IsClosed)
            {
                _channel = _connectionFactory.CreateChannel();
            }
        }

        /// <summary>
        /// Disposes the RabbitMQ channel
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the RabbitMQ channel
        /// </summary>
        /// <param name="disposing">True if disposing; otherwise, false</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed resources
                if (_channel != null && _channel.IsOpen)
                {
                    _channel.Close();
                    _channel.Dispose();
                    _channel = null;
                }
            }

            _disposed = true;
        }
    }
}