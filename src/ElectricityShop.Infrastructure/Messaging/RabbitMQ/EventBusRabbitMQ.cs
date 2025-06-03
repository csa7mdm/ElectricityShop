using ElectricityShop.Infrastructure.Messaging.Events;
using Microsoft.Extensions.Logging;
using global::RabbitMQ.Client;
using global::RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ElectricityShop.Infrastructure.Messaging.RabbitMQ
{
    public class EventBusRabbitMQ : IDisposable
    {
        private readonly RabbitMQConnection _connection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private global::RabbitMQ.Client.IModel _channel;
        private bool _disposed;

        public EventBusRabbitMQ(RabbitMQConnection connection, ILogger<EventBusRabbitMQ> logger)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (_connection.IsConnected)
            {
                _channel = _connection.CreateChannel();
                return;
            }

            await _connection.TryConnectAsync(cancellationToken);
            _channel = _connection.CreateChannel();
        }

        public void PublishMessage<T>(string routingKey, T message) where T : IntegrationEvent
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            var body = JsonSerializer.SerializeToUtf8Bytes(message);

            _logger.LogInformation("Publishing message to exchange 'electricity_shop_events' with routing key {RoutingKey}", routingKey);

            _channel.BasicPublish(
                "electricity_shop_events",
                routingKey,
                properties,
                body);
        }

        public void SubscribeAsync<T>(string queueName, string routingKey, Func<T, Task> handler) where T : IntegrationEvent
        {
            _logger.LogInformation("Declaring queue {QueueName}", queueName);

            _channel.ExchangeDeclare("electricity_shop_events", "topic", true);
            _channel.QueueDeclare(queueName, true, false, false);
            _channel.QueueBind(queueName, "electricity_shop_events", routingKey);

            _logger.LogInformation("Starting consumer for queue {QueueName}", queueName);

            var consumer = new CustomAsyncConsumer<T>(_channel, _logger, handler);
            _channel.BasicConsume(queueName, false, consumer);
        }

        private class CustomAsyncConsumer<T> : AsyncEventingBasicConsumer where T : IntegrationEvent
        {
            private readonly ILogger _logger;
            private readonly Func<T, Task> _handler;

            public CustomAsyncConsumer(
                global::RabbitMQ.Client.IModel model,
                ILogger logger,
                Func<T, Task> handler) : base(model)
            {
                _logger = logger;
                _handler = handler;
            }

            public override async Task HandleBasicDeliver(
                string consumerTag,
                ulong deliveryTag,
                bool redelivered,
                string exchange,
                string routingKey,
                IBasicProperties properties,
                ReadOnlyMemory<byte> body)
            {
                try
                {
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    var data = JsonSerializer.Deserialize<T>(message);

                    if (data != null)
                    {
                        await _handler(data);
                        Model.BasicAck(deliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                    Model.BasicNack(deliveryTag, false, true);
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            try
            {
                _channel?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error disposing RabbitMQ channel");
            }
            finally
            {
                _disposed = true;
            }
        }
    }
}