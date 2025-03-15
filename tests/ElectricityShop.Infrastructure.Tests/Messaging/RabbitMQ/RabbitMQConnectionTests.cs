using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Xunit;
using ElectricityShop.Infrastructure.Messaging.RabbitMQ;

namespace ElectricityShop.Infrastructure.Tests.Messaging.RabbitMQ
{
    public class RabbitMQConnectionTests
    {
        private readonly Mock<ConnectionFactory> _connectionFactoryMock;
        private readonly Mock<IConnection> _connectionMock;
        private readonly Mock<IChannel> _channelMock;
        private readonly Mock<ILogger<RabbitMQConnection>> _loggerMock;
        
        public RabbitMQConnectionTests()
        {
            _connectionFactoryMock = new Mock<ConnectionFactory>();
            _connectionMock = new Mock<IConnection>();
            _channelMock = new Mock<IChannel>();
            _loggerMock = new Mock<ILogger<RabbitMQConnection>>();
            
            // Setup connection factory to return our connection mock
            _connectionFactoryMock.Setup(cf => cf.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_connectionMock.Object);
                
            // Setup connection to return our channel mock
            _connectionMock.Setup(c => c.CreateChannelAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_channelMock.Object);
                
            // Setup default connection properties
            _connectionMock.Setup(c => c.IsOpen).Returns(true);
            _connectionMock.SetupGet(c => c.Endpoint).Returns(new AmqpTcpEndpoint("localhost"));
        }
        
        [Fact]
        public void Constructor_ValidatesParameters()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RabbitMQConnection(null, _loggerMock.Object));
            Assert.Throws<ArgumentNullException>(() => new RabbitMQConnection(_connectionFactoryMock.Object, null));
            
            // Valid construction should not throw
            var connection = new RabbitMQConnection(_connectionFactoryMock.Object, _loggerMock.Object);
            Assert.NotNull(connection);
        }
        
        [Fact]
        public async Task TryConnectAsync_Success_ReturnsTrue()
        {
            // Arrange
            var connection = new RabbitMQConnection(_connectionFactoryMock.Object, _loggerMock.Object);
            
            // Act
            var result = await connection.TryConnectAsync();
            
            // Assert
            Assert.True(result);
            _connectionFactoryMock.Verify(cf => cf.CreateConnectionAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.True(connection.IsConnected);
        }
        
        [Fact]
        public async Task TryConnectAsync_ConnectionFailure_ReturnsFalse()
        {
            // Arrange
            _connectionFactoryMock.Setup(cf => cf.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BrokerUnreachableException(new Exception("Connection failed")));
                
            var connection = new RabbitMQConnection(_connectionFactoryMock.Object, _loggerMock.Object);
            
            // Act
            var result = await connection.TryConnectAsync();
            
            // Assert
            Assert.False(result);
            Assert.False(connection.IsConnected);
        }
        
        [Fact]
        public async Task TryConnectAsync_ConnectionNotOpen_ReturnsFalse()
        {
            // Arrange
            _connectionMock.Setup(c => c.IsOpen).Returns(false);
            var connection = new RabbitMQConnection(_connectionFactoryMock.Object, _loggerMock.Object);
            
            // Act
            var result = await connection.TryConnectAsync();
            
            // Assert
            Assert.False(result);
            Assert.False(connection.IsConnected);
        }
        
        [Fact]
        public async Task CreateChannelAsync_ConnectionExists_ReturnsChannel()
        {
            // Arrange
            var connection = new RabbitMQConnection(_connectionFactoryMock.Object, _loggerMock.Object);
            await connection.TryConnectAsync(); // Establish connection first
            
            // Act
            var channel = await connection.CreateChannelAsync();
            
            // Assert
            Assert.NotNull(channel);
            _connectionMock.Verify(c => c.CreateChannelAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task CreateChannelAsync_NoConnection_AttemptsToConnect()
        {
            // Arrange
            var connection = new RabbitMQConnection(_connectionFactoryMock.Object, _loggerMock.Object);
            
            // Act
            var channel = await connection.CreateChannelAsync();
            
            // Assert
            Assert.NotNull(channel);
            _connectionFactoryMock.Verify(cf => cf.CreateConnectionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _connectionMock.Verify(c => c.CreateChannelAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task CreateChannelAsync_ConnectionFailure_ThrowsException()
        {
            // Arrange
            _connectionFactoryMock.Setup(cf => cf.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BrokerUnreachableException(new Exception("Connection failed")));
                
            var connection = new RabbitMQConnection(_connectionFactoryMock.Object, _loggerMock.Object);
            
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => connection.CreateChannelAsync());
        }
        
        [Fact]
        public async Task ConnectionShutdown_LogsWarning()
        {
            // Arrange
            var connection = new RabbitMQConnection(_connectionFactoryMock.Object, _loggerMock.Object);
            await connection.TryConnectAsync();
            
            // Capture the event handler
            EventHandler<ShutdownEventArgs> shutdownHandler = null;
            _connectionMock.Setup(c => c.ConnectionShutdown += It.IsAny<EventHandler<ShutdownEventArgs>>())
                .Callback<EventHandler<ShutdownEventArgs>>((handler) => shutdownHandler = handler);
            
            // Act - Trigger the shutdown event
            shutdownHandler?.Invoke(_connectionMock.Object, new ShutdownEventArgs(ShutdownInitiator.Application, 0, "Test shutdown"));
            
            // Assert - Verify that the logger was called with warning level
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("shutdown")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
        
        [Fact]
        public async Task DisposeAsync_ClosesConnection()
        {
            // Arrange
            _connectionMock.Setup(c => c.CloseAsync()).Returns(Task.CompletedTask);
            
            var connection = new RabbitMQConnection(_connectionFactoryMock.Object, _loggerMock.Object);
            await connection.TryConnectAsync();
            
            // Act
            await connection.DisposeAsync();
            
            // Assert
            _connectionMock.Verify(c => c.CloseAsync(), Times.Once);
        }
    }
}