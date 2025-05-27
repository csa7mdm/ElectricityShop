using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ElectricityShop.Infrastructure.Events
{
    /// <summary>
    /// Factory for creating RabbitMQ connections
    /// </summary>
    public class RabbitMqConnectionFactory : IDisposable
    {
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<RabbitMqConnectionFactory> _logger;
        private IConnection _connection;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqConnectionFactory"/> class
        /// </summary>
        /// <param name="options">The RabbitMQ settings options</param>
        /// <param name="logger">The logger</param>
        public RabbitMqConnectionFactory(
            IOptions<RabbitMqSettings> options,
            ILogger<RabbitMqConnectionFactory> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Gets a RabbitMQ connection, creating a new one if needed
        /// </summary>
        /// <returns>The RabbitMQ connection</returns>
        public IConnection GetConnection()
        {
            if (_connection != null && _connection.IsOpen)
            {
                return _connection;
            }

            _logger.LogInformation("Creating new RabbitMQ connection to {HostName}:{Port}",
                _settings.HostName, _settings.Port);

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(_settings.ConnectionTimeoutSeconds),
                // Automatic recovery if connection is lost
                AutomaticRecoveryEnabled = true,
                // Network recovery interval
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            if (_settings.UseSsl)
            {
                factory.Ssl = new SslOption { Enabled = true };
            }

            _connection = factory.CreateConnection();

            // Ensure the handler matches AsyncEventHandler<ShutdownEventArgs> and uses a valid property
            _connection.ConnectionShutdown += async (sender, args) =>
            {
                _logger.LogWarning("RabbitMQ connection shutdown. Reason: {Reason}", args.ReplyText); // Changed args.Reason to args.ReplyText
                await Task.CompletedTask; // Added to satisfy async Task return type
            };

            return _connection;
        }

        /// <summary>
        /// Creates a new channel from the connection
        /// </summary>
        /// <returns>A new RabbitMQ channel</returns>
        public IModel CreateChannel()
        {
            var connection = GetConnection();
            var channel = connection.CreateModel();

            // Set QoS (prefetch) to control how many messages are delivered at once
            channel.BasicQos(prefetchSize: 0, prefetchCount: _settings.PrefetchCount, global: false);

            return channel;
        }

        /// <summary>
        /// Disposes the RabbitMQ connection
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the RabbitMQ connection
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
                if (_connection != null && _connection.IsOpen)
                {
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
            }

            _disposed = true;
        }
    }
}