using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Domain.Events.Orders;
using ElectricityShop.Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using Xunit;

namespace ElectricityShop.API.Tests.Events
{
    public class RabbitMqEventBusTests
    {
        private readonly Mock<RabbitMqConnectionFactory> _connectionFactoryMock;
        private readonly Mock<IOptions<RabbitMqSettings>> _optionsMock;
        private readonly Mock<ILogger<RabbitMqEventBus>> _loggerMock;
        private readonly Mock<IModel> _channelMock;
        private readonly RabbitMqSettings _settings;
        private readonly RabbitMqEventBus _eventBus;

        public RabbitMqEventBusTests()
        {
            _connectionFactoryMock = new Mock<RabbitMqConnectionFactory>(
                Mock.Of<IOptions<RabbitMqSettings>>(),
                Mock.Of<ILogger<RabbitMqConnectionFactory>>());
            
            _settings = new RabbitMqSettings
            {
                ExchangeName = "test_exchange",
                DeadLetterExchange = "test_dead_letter_exchange"
            };
            
            _optionsMock = new Mock<IOptions<RabbitMqSettings>>();
            _optionsMock.Setup(o => o.Value).Returns(_settings);
            
            _loggerMock = new Mock<ILogger<RabbitMqEventBus>>();
            
            _channelMock = new Mock<IModel>();
            
            _connectionFactoryMock
                .Setup(cf => cf.CreateChannel())
                .Returns(_channelMock.Object);
            
            // Setup basic properties
            var propertiesMock = new Mock<IBasicProperties>();
            _channelMock
                .Setup(c => c.CreateBasicProperties())
                .Returns(propertiesMock.Object);
            
            _eventBus = new RabbitMqEventBus(
                _connectionFactoryMock.Object,
                _optionsMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task PublishAsync_WithValidEvent_ShouldPublishToExchange()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            
            var orderItems = new List<OrderItemDto>
            {
                new OrderItemDto
                {
                    ProductId = productId,
                    ProductName = "Product 1",
                    Quantity = 2,
                    UnitPrice = 100
                }
            };
            
            var totalAmount = 200;
            
            var @event = new OrderPlacedEvent(orderId, customerId, orderItems, totalAmount);

            // Act
            await _eventBus.PublishAsync(@event);

            // Assert
            _channelMock.Verify(
                c => c.BasicPublish(
                    _settings.ExchangeName,
                    "orderplacedevent",
                    true,
                    It.IsAny<IBasicProperties>(),
                    It.IsAny<ReadOnlyMemory<byte>>()),
                Times.Once);
        }

        [Fact]
        public async Task PublishAsync_WithNullEvent_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _eventBus.PublishAsync<OrderPlacedEvent>(null));
        }
    }
}