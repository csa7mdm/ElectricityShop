using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace ElectricityShop.Infrastructure.Messaging.RabbitMQ
{
    public class RabbitMQConnection : IAsyncDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMQConnection> _logger;
        private readonly int _retryCount;
        private IConnection? _connection;
        private bool _disposed;
        private readonly SemaphoreSlim _connectionSemaphore = new SemaphoreSlim(1, 1);

        public RabbitMQConnection(ConnectionFactory connectionFactory, ILogger<RabbitMQConnection> logger, int retryCount = 5)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryCount = retryCount;
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public async Task<bool> TryConnectAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            await _connectionSemaphore.WaitAsync(cancellationToken);
            try
            {
                if (IsConnected)
                {
                    return true;
                }

                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetryAsync(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                    });

                try
                {
                    return await policy.ExecuteAsync(async () =>
                    {
                        // Create connection asynchronously
                        // In version 7.1.1, the API for creating connections is different
                        // It takes various overloads but doesn't use an options object with CancellationToken
                        _connection = await _connectionFactory.CreateConnectionAsync();

                        if (IsConnected)
                        {
                            // We'll simply log connection status instead of using an event handler
                            // that might not exist in the current API version
                            _logger.LogInformation("RabbitMQ Client acquired a connection to '{HostName}'",
                                _connection.Endpoint.HostName);
                            return true;
                        }

                        _logger.LogCritical("FATAL ERROR: RabbitMQ connection could not be opened");
                        return false;
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "FATAL ERROR: RabbitMQ connection could not be created: {ExMessage}", ex.Message);
                    return false;
                }
            }
            finally
            {
                _connectionSemaphore.Release();
            }
        }

        public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
        {
            if (!IsConnected)
            {
                await TryConnectAsync(cancellationToken);
            }

            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            // In version 7.1.1, just call CreateChannelAsync without parameters
            return await _connection.CreateChannelAsync();
        }

        // Remove the unused method since we're not using ConnectionShutdown event anymore
        /*private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is shutdown. Reason: {0}", e.ReplyText);
        }*/

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                if (_connection != null && _connection.IsOpen)
                {
                    await _connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error during RabbitMQ connection disposal");
            }
        }
    }
}