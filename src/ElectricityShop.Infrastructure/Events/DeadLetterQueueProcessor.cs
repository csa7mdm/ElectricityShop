using ElectricityShop.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ElectricityShop.Infrastructure.Events
{
    /// <summary>
    /// Background service for processing messages from the dead letter queue
    /// </summary>
    public class DeadLetterQueueProcessor : BackgroundService
    {
        private readonly RabbitMqConnectionFactory _connectionFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<DeadLetterQueueProcessor> _logger;
        private IModel _channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeadLetterQueueProcessor"/> class
        /// </summary>
        /// <param name="connectionFactory">The RabbitMQ connection factory</param>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="options">The RabbitMQ settings options</param>
        /// <param name="logger">The logger</param>
        public DeadLetterQueueProcessor(
            RabbitMqConnectionFactory connectionFactory,
            IServiceProvider serviceProvider,
            IOptions<RabbitMqSettings> options,
            ILogger<DeadLetterQueueProcessor> logger)
        {
            _connectionFactory = connectionFactory;
            _serviceProvider = serviceProvider;
            _settings = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Starts the dead letter queue processor
        /// </summary>
        /// <param name="stoppingToken">The stopping token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            _channel = _connectionFactory.CreateChannel();

            // Set up error queue processing
            var eventTypeRegistry = _serviceProvider.GetRequiredService<EventTypeRegistry>();

            foreach (var registration in eventTypeRegistry.EventTypes)
            {
                var eventName = registration.Key;
                var routingKey = eventName.ToLower();
                var errorQueueName = $"electricity_shop.{routingKey}.error";

                // Start consuming from the error queue
                var consumer = new AsyncEventingBasicConsumer((IChannel)_channel);
                consumer.ReceivedAsync += async (model, ea) =>
                    await ProcessDeadLetterMessageAsync(ea, errorQueueName, stoppingToken);

                _channel.BasicConsume(queue: errorQueueName, autoAck: false, consumer: consumer);

                _logger.LogInformation("Started consuming messages from error queue {ErrorQueue}", errorQueueName);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Processes a message from the dead letter queue
        /// </summary>
        /// <param name="ea">The delivery event arguments</param>
        /// <param name="queueName">The queue name</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task ProcessDeadLetterMessageAsync(
            BasicDeliverEventArgs ea,
            string queueName,
            CancellationToken cancellationToken)
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var messageId = ea.BasicProperties.MessageId;
            var eventType = ea.BasicProperties.Type;

            _logger.LogWarning("Processing failed message {MessageId} of type {EventType} from queue {QueueName}",
                messageId, eventType, queueName);

            // Extract error information from headers
            var error = "Unknown error";
            if (ea.BasicProperties.Headers != null &&
                ea.BasicProperties.Headers.TryGetValue("x-error", out var errorObj))
            {
                error = Encoding.UTF8.GetString((byte[])errorObj);
            }

            // Extract original exchange and routing key
            var originalExchange = _settings.ExchangeName;
            var originalRoutingKey = string.Empty;

            if (ea.BasicProperties.Headers != null)
            {
                if (ea.BasicProperties.Headers.TryGetValue("x-original-exchange", out var exchangeObj))
                {
                    originalExchange = Encoding.UTF8.GetString((byte[])exchangeObj);
                }

                if (ea.BasicProperties.Headers.TryGetValue("x-original-routing-key", out var routingKeyObj))
                {
                    originalRoutingKey = Encoding.UTF8.GetString((byte[])routingKeyObj);
                }
            }

            // Log the failed message
            await using var scope = _serviceProvider.CreateAsyncScope();
            var failedMessageLogger = scope.ServiceProvider.GetService<IFailedMessageLogger>();

            if (failedMessageLogger != null)
            {
                await failedMessageLogger.LogFailedMessageAsync(
                    messageId,
                    eventType,
                    originalExchange,
                    originalRoutingKey,
                    message,
                    error,
                    DateTime.UtcNow,
                    cancellationToken);
            }

            // Acknowledge the message to remove it from the queue
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

            _logger.LogInformation("Failed message {MessageId} logged and removed from queue {QueueName}",
                messageId, queueName);
        }

        /// <summary>
        /// Stops the dead letter queue processor
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping dead letter queue processor");

            if (_channel != null && _channel.IsOpen)
            {
                _channel.Close();
                _channel.Dispose();
            }

            return base.StopAsync(cancellationToken);
        }
    }
}