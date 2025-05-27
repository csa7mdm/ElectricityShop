using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Common.Events;
using ElectricityShop.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ElectricityShop.Infrastructure.Events
{
    /// <summary>
    /// RabbitMQ event consumer for handling domain events
    /// </summary>
    public class RabbitMqEventConsumer : BackgroundService
    {
        private readonly RabbitMqConnectionFactory _connectionFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMqSettings _settings;
        private readonly Dictionary<string, Type> _eventTypes;
        private readonly ILogger<RabbitMqEventConsumer> _logger;
        private IModel _channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqEventConsumer"/> class
        /// </summary>
        /// <param name="connectionFactory">The RabbitMQ connection factory</param>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="options">The RabbitMQ settings options</param>
        /// <param name="eventTypeRegistry">The event type registry</param>
        /// <param name="logger">The logger</param>
        public RabbitMqEventConsumer(
            RabbitMqConnectionFactory connectionFactory,
            IServiceProvider serviceProvider,
            IOptions<RabbitMqSettings> options,
            EventTypeRegistry eventTypeRegistry,
            ILogger<RabbitMqEventConsumer> logger)
        {
            _connectionFactory = connectionFactory;
            _serviceProvider = serviceProvider;
            _settings = options.Value;
            _eventTypes = eventTypeRegistry.EventTypes;
            _logger = logger;
        }

        /// <summary>
        /// Starts the consumer
        /// </summary>
        /// <param name="stoppingToken">The stopping token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            _channel = _connectionFactory.CreateChannel();

            // Create the main queue with dead-letter configuration
            foreach (var registration in _eventTypes)
            {
                var eventName = registration.Key;
                var routingKey = eventName.ToLower();
                var queueName = $"electricity_shop.{routingKey}";
                var retryQueueName = $"electricity_shop.{routingKey}.retry";
                var errorQueueName = $"electricity_shop.{routingKey}.error";

                // Setup dead letter and retry infrastructure
                SetupEventQueues(queueName, retryQueueName, errorQueueName, routingKey);

                // Start consuming from the main queue
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) => 
                    await ProcessMessageAsync(ea, queueName, retryQueueName, errorQueueName, routingKey, stoppingToken);

                _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                
                _logger.LogInformation("Started consuming messages from queue {QueueName} with routing key {RoutingKey}", 
                    queueName, routingKey);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Processes a message from the queue
        /// </summary>
        /// <param name="ea">The delivery event arguments</param>
        /// <param name="queueName">The queue name</param>
        /// <param name="retryQueueName">The retry queue name</param>
        /// <param name="errorQueueName">The error queue name</param>
        /// <param name="routingKey">The routing key</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task ProcessMessageAsync(
            BasicDeliverEventArgs ea, 
            string queueName,
            string retryQueueName,
            string errorQueueName,
            string routingKey,
            CancellationToken cancellationToken)
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var messageId = ea.BasicProperties.MessageId;
            var eventType = ea.BasicProperties.Type;
            
            _logger.LogInformation("Processing message {MessageId} of type {EventType}", messageId, eventType);

            try
            {
                if (_eventTypes.TryGetValue(eventType, out var type))
                {
                    var @event = (IDomainEvent)JsonSerializer.Deserialize(message, type);
                    
                    // Extract retry count from headers
                    var retryCount = 0;
                    if (ea.BasicProperties.Headers != null && 
                        ea.BasicProperties.Headers.TryGetValue("x-retry-count", out var retryObj))
                    {
                        retryCount = Convert.ToInt32(retryObj);
                    }

                    await using var scope = _serviceProvider.CreateAsyncScope();
                    var handlerType = typeof(IEventHandler<>).MakeGenericType(type);
                    var handlers = scope.ServiceProvider.GetServices(handlerType);

                    foreach (var handler in handlers)
                    {
                        var handleMethod = handlerType.GetMethod("HandleAsync");
                        
                        _logger.LogDebug("Invoking handler {HandlerType} for message {MessageId}", 
                            handler.GetType().Name, messageId);
                        
                        await (Task)handleMethod.Invoke(handler, new object[] { @event, cancellationToken });
                    }

                    // Acknowledge the message after successful processing
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    
                    _logger.LogInformation("Successfully processed message {MessageId} of type {EventType}", 
                        messageId, eventType);
                }
                else
                {
                    _logger.LogWarning("No handler registered for event type {EventType}", eventType);
                    
                    // No handler registered, move to error queue
                    PublishToErrorQueue(ea, errorQueueName, "No handler registered for this event type");
                    
                    // Acknowledge the message to remove it from the original queue
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message {MessageId} of type {EventType}: {ErrorMessage}", 
                    messageId, eventType, ex.Message);

                // Extract retry count from headers
                var retryCount = 0;
                if (ea.BasicProperties.Headers != null && 
                    ea.BasicProperties.Headers.TryGetValue("x-retry-count", out var retryObj))
                {
                    retryCount = Convert.ToInt32(retryObj);
                }

                // Check if we should retry or move to error queue
                if (retryCount < _settings.RetryCount)
                {
                    // Increment retry count and publish to retry queue
                    PublishToRetryQueue(ea, retryQueueName, retryCount + 1, ex.Message);
                    
                    // Acknowledge the message to remove it from the original queue
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                else
                {
                    // Max retries reached, move to error queue
                    PublishToErrorQueue(ea, errorQueueName, ex.Message);
                    
                    // Acknowledge the message to remove it from the original queue
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            }
        }

        /// <summary>
        /// Sets up the event queues for a specific event type
        /// </summary>
        /// <param name="queueName">The main queue name</param>
        /// <param name="retryQueueName">The retry queue name</param>
        /// <param name="errorQueueName">The error queue name</param>
        /// <param name="routingKey">The routing key</param>
        private void SetupEventQueues(
            string queueName, 
            string retryQueueName, 
            string errorQueueName, 
            string routingKey)
        {
            // Declare the error queue
            _channel.QueueDeclare(
                queue: errorQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueBind(
                queue: errorQueueName,
                exchange: _settings.DeadLetterExchange,
                routingKey: $"{routingKey}.error");

            // Declare the retry queue with TTL (delay before requeuing)
            var retryQueueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", _settings.ExchangeName },
                { "x-dead-letter-routing-key", routingKey },
                { "x-message-ttl", _settings.RetryIntervalSeconds * 1000 } // TTL in milliseconds
            };

            _channel.QueueDeclare(
                queue: retryQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: retryQueueArgs);

            _channel.QueueBind(
                queue: retryQueueName,
                exchange: _settings.DeadLetterExchange,
                routingKey: $"{routingKey}.retry");

            // Declare the main queue with dead-letter configuration
            var queueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", _settings.DeadLetterExchange },
                { "x-dead-letter-routing-key", $"{routingKey}.error" }
            };

            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArgs);

            _channel.QueueBind(
                queue: queueName,
                exchange: _settings.ExchangeName,
                routingKey: routingKey);
        }

        /// <summary>
        /// Publishes a message to the retry queue
        /// </summary>
        /// <param name="ea">The original delivery event arguments</param>
        /// <param name="retryQueueName">The retry queue name</param>
        /// <param name="retryCount">The retry count</param>
        /// <param name="errorMessage">The error message</param>
        private void PublishToRetryQueue(
            BasicDeliverEventArgs ea, 
            string retryQueueName, 
            int retryCount, 
            string errorMessage)
        {
            var properties = _channel.CreateBasicProperties();
            properties.DeliveryMode = 2; // persistent
            properties.ContentType = ea.BasicProperties.ContentType;
            properties.MessageId = ea.BasicProperties.MessageId;
            properties.Type = ea.BasicProperties.Type;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            
            // Copy existing headers if present
            properties.Headers = ea.BasicProperties.Headers != null 
                ? new Dictionary<string, object>(ea.BasicProperties.Headers) 
                : new Dictionary<string, object>();
            
            // Update or add retry count
            properties.Headers["x-retry-count"] = retryCount;
            properties.Headers["x-last-error"] = errorMessage;
            properties.Headers["x-original-exchange"] = ea.Exchange;
            properties.Headers["x-original-routing-key"] = ea.RoutingKey;

            // Publish to retry queue (will be delayed by TTL)
            _channel.BasicPublish(
                exchange: _settings.DeadLetterExchange,
                routingKey: $"{ea.RoutingKey}.retry",
                mandatory: true,
                basicProperties: properties,
                body: ea.Body);
            
            _logger.LogInformation("Published message {MessageId} to retry queue {RetryQueue} (retry count: {RetryCount})", 
                properties.MessageId, retryQueueName, retryCount);
        }

        /// <summary>
        /// Publishes a message to the error queue
        /// </summary>
        /// <param name="ea">The original delivery event arguments</param>
        /// <param name="errorQueueName">The error queue name</param>
        /// <param name="errorMessage">The error message</param>
        private void PublishToErrorQueue(
            BasicDeliverEventArgs ea, 
            string errorQueueName, 
            string errorMessage)
        {
            var properties = _channel.CreateBasicProperties();
            properties.DeliveryMode = 2; // persistent
            properties.ContentType = ea.BasicProperties.ContentType;
            properties.MessageId = ea.BasicProperties.MessageId;
            properties.Type = ea.BasicProperties.Type;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            
            // Copy existing headers if present
            properties.Headers = ea.BasicProperties.Headers != null 
                ? new Dictionary<string, object>(ea.BasicProperties.Headers) 
                : new Dictionary<string, object>();
            
            // Add error information
            properties.Headers["x-error"] = errorMessage;
            properties.Headers["x-original-exchange"] = ea.Exchange;
            properties.Headers["x-original-routing-key"] = ea.RoutingKey;
            properties.Headers["x-failed-at"] = DateTime.UtcNow.ToString("o");

            // Publish to error queue
            _channel.BasicPublish(
                exchange: _settings.DeadLetterExchange,
                routingKey: $"{ea.RoutingKey}.error",
                mandatory: true,
                basicProperties: properties,
                body: ea.Body);
            
            _logger.LogWarning("Published message {MessageId} to error queue {ErrorQueue} due to error: {ErrorMessage}", 
                properties.MessageId, errorQueueName, errorMessage);
        }

        /// <summary>
        /// Stops the consumer
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping RabbitMQ event consumer");
            
            if (_channel != null && _channel.IsOpen)
            {
                _channel.Close();
                _channel.Dispose();
            }
            
            return base.StopAsync(cancellationToken);
        }
    }
}