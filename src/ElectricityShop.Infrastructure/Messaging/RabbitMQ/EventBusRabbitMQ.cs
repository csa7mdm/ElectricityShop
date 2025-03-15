using ElectricityShop.Infrastructure.Messaging.Events;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ElectricityShop.Infrastructure.Messaging.RabbitMQ
{
    public class EventBusRabbitMQ : IAsyncDisposable
    {
        private readonly RabbitMQConnection _connection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly string _exchangeName;
        private IChannel? _channel;
        private readonly SemaphoreSlim _channelSemaphore = new SemaphoreSlim(1, 1);

        public EventBusRabbitMQ(RabbitMQConnection connection, ILogger<EventBusRabbitMQ> logger, string exchangeName = "electricity_shop_exchange")
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _exchangeName = exchangeName;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await EnsureChannelCreatedAsync(cancellationToken);
    }

    private async Task EnsureChannelCreatedAsync(CancellationToken cancellationToken = default)
        {
            if (_channel == null || _channel.IsClosed)
            {
                await _channelSemaphore.WaitAsync(cancellationToken);
                try
                {
                    if (_channel == null || _channel.IsClosed)
                    {
                        if (!_connection.IsConnected)
                        {
                            await _connection.TryConnectAsync(cancellationToken);
                        }

                        _channel = await _connection.CreateChannelAsync(cancellationToken);

                        // Declare the exchange
                        await _channel.ExchangeDeclareAsync(
                            exchange: _exchangeName,
                            type: ExchangeType.Direct,
                            durable: true,
                            autoDelete: false,
                            arguments: null,
                            cancellationToken: cancellationToken);
                    }
                }
                finally
                {
                    _channelSemaphore.Release();
                }
            }
        }

        public async Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            await EnsureChannelCreatedAsync(cancellationToken);

            var messageJson = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            // Create basic properties - now just instantiate the class directly
            var properties = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent, // Use enum instead of int
                ContentType = "application/json"
            };

            _logger.LogInformation("Publishing message to {Exchange} with routing key {RoutingKey}", _exchangeName, routingKey);

            // Publish message
            await _channel.BasicPublishAsync(
                exchange: _exchangeName,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }

        public async Task SubscribeAsync<T>(string queueName, string routingKey, Func<T, Task> handler, CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            await EnsureChannelCreatedAsync(cancellationToken);

            _logger.LogInformation("Declaring queue {QueueName}", queueName);

            // Declare queue
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            // Bind queue to exchange
            await _channel.QueueBindAsync(
                queue: queueName,
                exchange: _exchangeName,
                routingKey: routingKey,
                arguments: null,
                cancellationToken: cancellationToken);

            // Set quality of service - prefetch only one message at a time
            await _channel.BasicQosAsync(0, 1, false, cancellationToken);

            _logger.LogInformation("Starting consumer for queue {QueueName}", queueName);

            // Create a custom implementation of IAsyncBasicConsumer that implements the interface directly
            var consumer = new CustomAsyncConsumer<T>(_channel, handler, _logger);

            // Start consuming from the queue
            await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken);
        }

        // Custom consumer implementation that directly implements IAsyncBasicConsumer
        private class CustomAsyncConsumer<T> : IAsyncBasicConsumer where T : IntegrationEvent
        {
            private readonly IChannel _channel;
            private readonly Func<T, Task> _handler;
            private readonly ILogger _logger;

            public CustomAsyncConsumer(IChannel channel, Func<T, Task> handler, ILogger logger)
            {
                _channel = channel;
                _handler = handler;
                _logger = logger;
            }

            public IChannel Channel => _channel;

            public event AsyncEventHandler<ConsumerEventArgs> ConsumerCancelled;

            public Task HandleBasicCancelAsync(string consumerTag, CancellationToken cancellationToken)
            {
                var handler = ConsumerCancelled;
                if (handler != null)
                {
                    // Create a ConsumerEventArgs with an array of strings containing consumerTag
                    return handler(this, new ConsumerEventArgs(new[] { consumerTag }));
                }
                return Task.CompletedTask;
            }

            public Task HandleBasicCancelOkAsync(string consumerTag, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public Task HandleBasicConsumeOkAsync(string consumerTag, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public async Task HandleBasicDeliverAsync(
                string consumerTag,
                ulong deliveryTag,
                bool redelivered,
                string exchange,
                string routingKey,
                IReadOnlyBasicProperties properties,
                ReadOnlyMemory<byte> body,
                CancellationToken cancellationToken)
            {
                try
                {
                    // Important: Copy the body to a new array for safety
                    var bodyArray = body.ToArray();
                    var content = Encoding.UTF8.GetString(bodyArray);

                    var message = JsonSerializer.Deserialize<T>(content);

                    // Process the message
                    await _handler(message);

                    // Acknowledge successful processing
                    await _channel.BasicAckAsync(deliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");

                    // Negative acknowledgment with requeue
                    await _channel.BasicNackAsync(deliveryTag, false, true);
                }
            }

            public Task HandleChannelShutdownAsync(object sender, ShutdownEventArgs e)
            {
                return Task.CompletedTask;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null && _channel.IsOpen)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }
        }
    }
}