using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using global::RabbitMQ.Client;

namespace ElectricityShop.Infrastructure.Messaging.RabbitMQ
{
    public class RabbitMQConnection : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMQConnection> _logger;
        private IConnection _connection;
        private bool _disposed;

        public RabbitMQConnection(ConnectionFactory connectionFactory, ILogger<RabbitMQConnection> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public async Task<bool> TryConnectAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                _connection = _connectionFactory.CreateConnection();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not connect to RabbitMQ");
                return false;
            }
        }

        public IModel CreateChannel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to create channel");
            }

            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;

            try
            {
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error disposing RabbitMQ connection");
            }
            finally
            {
                _disposed = true;
            }
        }
    }
}