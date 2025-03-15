using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Infrastructure.Messaging.Events;
using ElectricityShop.Infrastructure.Messaging.RabbitMQ;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;

namespace ElectricityShop.Infrastructure.Tests.Messaging.RabbitMQ
{
    public class EventBusRabbitMQTests
    {
        private readonly Mock<RabbitMQConnection> _connectionMock;
        private readonly Mock<IChannel> _channelMock;
        private readonly Mock<ILogger<EventBusRabbitMQ>> _loggerMock;
        private readonly Mock<IBasicProperties> _propertiesMock;
        private readonly string _exchangeName = "test_exchange";
        
        public EventBusRabbitMQTests()
        {
            _connectionMock = new Mock<RabbitMQConnection>(null, null);
            _channelMock = new Mock<IChannel>();
            _loggerMock = new Mock<ILogger<EventBusRabbitMQ>>();
            _propertiesMock = new Mock<IBasicProperties>();
            
            // Setup connection to be connected and return our channel mock
            _connectionMock.Setup(c => c.IsConnected).Returns(true);
            _connectionMock.Setup(c => c.CreateChannelAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_channelMock.Object);
                
            // Setup channel for basic operations
            _channelMock.Setup(c => c.CreateBasicProperties())
                .Returns(_propertiesMock.Object);
            _channelMock.Setup(c => c.ExchangeDeclareAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<bool>(), 
                It.IsAny<IDictionary<string, object>>(), 
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _channelMock.Setup(c => c.QueueDeclareAsync(
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<bool>(), 
                It.IsAny<bool>(), 
                It.IsAny<IDictionary<string, object>>(), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QueueDeclareOk("test_queue", 0, 0));
            _channelMock.Setup(c => c.QueueBindAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<IDictionary<string, object>>(), 
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _channelMock.Setup(c => c.BasicPublishAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<IBasicProperties>(), 
                It.IsAny<ReadOnlyMemory<byte>>(), 
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _channelMock.Setup(c => c.BasicConsumeAsync(
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<bool>(), 
                It.IsAny<IDictionary<string, object>>(), 
                It.IsAny<IBasicConsumer>(), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync("consumer_tag");
                
            // Setup channel open status
            _channelMock.Setup(c => c.IsClosed).Returns(false);
        }
        
        [Fact]
        public void Constructor_ValidatesParameters()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new EventBusRabbitMQ(null, _loggerMock.Object));
            Assert.Throws<ArgumentNullException>(() => new EventBusRabbitMQ(_connectionMock.Object, null));
            
            // Valid construction should not throw
            var eventBus = new EventBusRabbitMQ(_connectionMock.Object, _loggerMock.Object, _exchangeName);
            Assert.NotNull(eventBus);
        }
        
        [Fact]
        public async Task EnsureChannelCreatedAsync_NoChannel_CreatesNewChannel()
        {
            // Arrange
            var eventBus = new EventBusRabbitMQ(_connectionMock.Object, _loggerMock.Object, _exchangeName);
            
            // Use reflection to access the private method
            var method = typeof(EventBusRabbitMQ).GetMethod("EnsureChannelCreatedAsync", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Act
            await (Task)method.Invoke(eventBus, new object[] { CancellationToken.None });
            
            // Assert
            _connectionMock.Verify(c => c.CreateChannelAsync(It.IsAny<CancellationToken>()), Times.Once);
            _channelMock.Verify(c => c.ExchangeDeclareAsync(
                _exchangeName, 
                ExchangeType.Direct, 
                true, 
                false, 
                null, 
                It.IsAny<CancellationToken>()), 
                Times.Once);
        }
        
        [Fact]
        public async Task PublishAsync_PublishesMessage()
        {
            // Arrange
            var eventBus = new EventBusRabbitMQ(_connectionMock.Object, _loggerMock.Object, _exchangeName);
            var testEvent = new TestEvent { Id = Guid.NewGuid(), TestProperty = "Test Value" };
            var routingKey = "test_routing_key";
            
            // Act
            await eventBus.PublishAsync(testEvent, routingKey);
            
            // Assert
            _channelMock.Verify(c => c.BasicPublishAsync(
                _exchangeName,
                routingKey,
                It.IsAny<bool>(),
                It.IsAny<IBasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async Task PublishAsync_SetsCorrectMessageProperties()
        {
            // Arrange
            var eventBus = new EventBusRabbitMQ(_connectionMock.Object, _loggerMock.Object, _exchangeName);
            var testEvent = new TestEvent { Id = Guid.NewGuid(), TestProperty = "Test Value" };
            var routingKey = "test_routing_key";
            
            // Setup property verification
            _propertiesMock.SetupSet(p => p.DeliveryMode = 2).Verifiable();
            _propertiesMock.SetupSet(p => p.ContentType = "application/json").Verifiable();
            
            // Act
            await eventBus.PublishAsync(testEvent, routingKey);
            
            // Assert
            _propertiesMock.Verify();
        }
        
        [Fact]
        public async Task SubscribeAsync_RegistersConsumer()
        {
            // Arrange
            var eventBus = new EventBusRabbitMQ(_connectionMock.Object, _loggerMock.Object, _exchangeName);
            var queueName = "test_queue";
            var routingKey = "test_routing_key";
            
            Func<TestEvent, Task> handler = _ => Task.CompletedTask;
            
            // Act
            await eventBus.SubscribeAsync(queueName, routingKey, handler);
            
            // Assert
            _channelMock.Verify(c => c.QueueDeclareAsync(
                queueName,
                true,
                false,
                false,
                null,
                It.IsAny<CancellationToken>()),
                Times.Once);
                
            _channelMock.Verify(c => c.QueueBindAsync(
                queueName,
                _exchangeName,
                routingKey,
                null,
                It.IsAny<CancellationToken>()),
                Times.Once);
                
            _channelMock.Verify(c => c.BasicConsumeAsync(
                queueName,
                false,
                It.IsAny<IBasicConsumer>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async Task SubscribeAsync_MessageReceived_ProcessesMessage()
        {
            // Arrange
            var eventBus = new EventBusRabbitMQ(_connectionMock.Object, _loggerMock.Object, _exchangeName);
            var queueName = "test_queue";
            var routingKey = "test_routing_key";
            
            var handlerCalled = false;
            Func<TestEvent, Task> handler = _ => {
                handlerCalled = true;
                return Task.CompletedTask;
            };
            
            // Capture the consumer to manually trigger the Received event
            AsyncEventingBasicConsumer capturedConsumer = null;
            _channelMock.Setup(c => c.BasicConsumeAsync(
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<IBasicConsumer>(),
                It.IsAny<CancellationToken>()))
                .Callback<string, bool, IBasicConsumer, CancellationToken>((_, _, consumer, _) => 
                {
                    capturedConsumer = consumer as AsyncEventingBasicConsumer;
                })
                .ReturnsAsync("consumer_tag");
                
            // Setup channel to handle Ack
            _channelMock.Setup(c => c.BasicAckAsync(It.IsAny<ulong>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            
            // Act
            await eventBus.SubscribeAsync(queueName, routingKey, handler);
            
            // Make sure we captured the consumer
            Assert.NotNull(capturedConsumer);
            
            // Create a test message
            var testEvent = new TestEvent { Id = Guid.NewGuid(), TestProperty = "Test Value" };
            var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(testEvent));
            var eventArgs = new BasicDeliverEventArgs
            {
                DeliveryTag = 1,
                RoutingKey = routingKey,
                Exchange = _exchangeName,
                BasicProperties = _propertiesMock.Object,
                Body = new ReadOnlyMemory<byte>(messageBytes)
            };
            
            // Trigger the event
            await capturedConsumer.HandleBasicDeliver(
                "consumer_tag", 
                eventArgs.DeliveryTag, 
                false, 
                eventArgs.Exchange, 
                eventArgs.RoutingKey, 
                eventArgs.BasicProperties, 
                eventArgs.Body);
            
            // Assert
            Assert.True(handlerCalled);
            _channelMock.Verify(c => c.BasicAckAsync(
                eventArgs.DeliveryTag,
                false,
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async Task SubscribeAsync_HandlerThrowsException_NacksMessage()
        {
            // Arrange
            var eventBus = new EventBusRabbitMQ(_connectionMock.Object, _loggerMock.Object, _exchangeName);
            var queueName = "test_queue";
            var routingKey = "test_routing_key";
            
            Func<TestEvent, Task> handler = _ => throw new Exception("Test exception");
            
            // Capture the consumer to manually trigger the Received event
            AsyncEventingBasicConsumer capturedConsumer = null;
            _channelMock.Setup(c => c.BasicConsumeAsync(
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<IBasicConsumer>(),
                It.IsAny<CancellationToken>()))
                .Callback<string, bool, IBasicConsumer, CancellationToken>((_, _, consumer, _) => 
                {
                    capturedConsumer = consumer as AsyncEventingBasicConsumer;
                })
                .ReturnsAsync("consumer_tag");
                
            // Setup channel to handle Nack
            _channelMock.Setup(c => c.BasicNackAsync(It.IsAny<ulong>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            
            // Act
            await eventBus.SubscribeAsync(queueName, routingKey, handler);
            
            // Make sure we captured the consumer
            Assert.NotNull(capturedConsumer);
            
            // Create a test message
            var testEvent = new TestEvent { Id = Guid.NewGuid(), TestProperty = "Test Value" };
            var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(testEvent));
            var eventArgs = new BasicDeliverEventArgs
            {
                DeliveryTag = 1,
                RoutingKey = routingKey,
                Exchange = _exchangeName,
                BasicProperties = _propertiesMock.Object,
                Body = new ReadOnlyMemory<byte>(messageBytes)
            };
            
            // Trigger the event
            await capturedConsumer.HandleBasicDeliver(
                "consumer_tag", 
                eventArgs.DeliveryTag, 
                false, 
                eventArgs.Exchange, 
                eventArgs.RoutingKey, 
                eventArgs.BasicProperties, 
                eventArgs.Body);
            
            // Assert
            _channelMock.Verify(c => c.BasicNackAsync(
                eventArgs.DeliveryTag,
                false,
                true,
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async Task DisposeAsync_ClosesChannel()
        {
            // Arrange
            _channelMock.Setup(c => c.CloseAsync()).Returns(Task.CompletedTask);
            _channelMock.Setup(c => c.IsOpen).Returns(true);
            
            var eventBus = new EventBusRabbitMQ(_connectionMock.Object, _loggerMock.Object, _exchangeName);
            
            // Need to initialize the channel first
            await eventBus.PublishAsync(new TestEvent(), "test");
            
            // Act
            await eventBus.DisposeAsync();
            
            // Assert
            _channelMock.Verify(c => c.CloseAsync(), Times.Once);
        }
    }
    
    // Helper class for testing
    public class TestEvent : IntegrationEvent
    {
        public string TestProperty { get; set; }
    }
}