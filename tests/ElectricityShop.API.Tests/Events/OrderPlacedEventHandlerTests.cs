using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Application.Orders.EventHandlers;
using ElectricityShop.Domain.Events.Orders;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ElectricityShop.API.Tests.Events
{
    public class OrderPlacedEventHandlerTests
    {
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IInventoryService> _inventoryServiceMock;
        private readonly Mock<ILogger<OrderPlacedEventHandler>> _loggerMock;
        private readonly OrderPlacedEventHandler _handler;

        public OrderPlacedEventHandlerTests()
        {
            _notificationServiceMock = new Mock<INotificationService>();
            _inventoryServiceMock = new Mock<IInventoryService>();
            _loggerMock = new Mock<ILogger<OrderPlacedEventHandler>>();

            _handler = new OrderPlacedEventHandler(
                _notificationServiceMock.Object,
                _inventoryServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task HandleAsync_ShouldDeductInventoryAndSendNotification()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();
            
            var orderItems = new List<OrderItemDto>
            {
                new OrderItemDto
                {
                    ProductId = productId1,
                    ProductName = "Product 1",
                    Quantity = 2,
                    UnitPrice = 100
                },
                new OrderItemDto
                {
                    ProductId = productId2,
                    ProductName = "Product 2",
                    Quantity = 1,
                    UnitPrice = 50
                }
            };
            
            var totalAmount = 250;
            
            var @event = new OrderPlacedEvent(orderId, customerId, orderItems, totalAmount);

            // Act
            await _handler.HandleAsync(@event, CancellationToken.None);

            // Assert
            _inventoryServiceMock.Verify(
                x => x.DeductInventoryAsync(productId1, 2, It.IsAny<CancellationToken>()),
                Times.Once);
            
            _inventoryServiceMock.Verify(
                x => x.DeductInventoryAsync(productId2, 1, It.IsAny<CancellationToken>()),
                Times.Once);
            
            _notificationServiceMock.Verify(
                x => x.SendOrderConfirmationAsync(
                    customerId, orderId, totalAmount, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenInventoryDeductionFails_ShouldThrowException()
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
            
            _inventoryServiceMock
                .Setup(x => x.DeductInventoryAsync(
                    productId, 2, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Not enough inventory"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.HandleAsync(@event, CancellationToken.None));
            
            // Should not send notification if inventory deduction fails
            _notificationServiceMock.Verify(
                x => x.SendOrderConfirmationAsync(
                    It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}