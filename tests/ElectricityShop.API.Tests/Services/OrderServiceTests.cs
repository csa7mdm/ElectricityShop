using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Common.Events;
using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Events.Orders;
using ElectricityShop.Domain.Interfaces;
using ElectricityShop.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ElectricityShop.API.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IEventBus> _eventBusMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly Mock<IRepository<Order>> _orderRepositoryMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _eventBusMock = new Mock<IEventBus>();
            _loggerMock = new Mock<ILogger<OrderService>>();
            _orderRepositoryMock = new Mock<IRepository<Order>>();
            
            _unitOfWorkMock
                .Setup(uow => uow.GetRepository<Order>())
                .Returns(_orderRepositoryMock.Object);
            
            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _eventBusMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldCreateOrderAndPublishEvent()
        {
            // Arrange
            var customerId = Guid.NewGuid();
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

            Order createdOrder = null;
            
            _orderRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Order>()))
                .Callback<Order>(o => createdOrder = o)
                .Returns(Task.CompletedTask);

            // Act
            var orderId = await _orderService.CreateOrderAsync(customerId, orderItems);

            // Assert
            Assert.NotEqual(Guid.Empty, orderId);
            
            _orderRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Order>()),
                Times.Once);
            
            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(),
                Times.Once);
            
            _eventBusMock.Verify(
                eb => eb.PublishAsync(
                    It.Is<OrderPlacedEvent>(e => 
                        e.OrderId == orderId && 
                        e.CustomerId == customerId && 
                        e.TotalAmount == 250),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            
            Assert.NotNull(createdOrder);
            Assert.Equal(customerId, createdOrder.CustomerId);
            Assert.Equal("Pending", createdOrder.Status);
            Assert.Equal(250, createdOrder.TotalAmount);
            Assert.Equal(2, createdOrder.Items.Count);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldUpdateStatusAndPublishEvent()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                CustomerId = customerId,
                Status = "Pending",
                OrderDate = DateTime.UtcNow,
                TotalAmount = 100
            };
            
            _orderRepositoryMock
                .Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync(order);
            
            var newStatus = "Processing";
            var updatedById = Guid.NewGuid();
            var notes = "Order is being processed";

            // Act
            await _orderService.UpdateOrderStatusAsync(orderId, newStatus, updatedById, notes);

            // Assert
            Assert.Equal(newStatus, order.Status);
            
            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(),
                Times.Once);
            
            _eventBusMock.Verify(
                eb => eb.PublishAsync(
                    It.Is<OrderStatusChangedEvent>(e => 
                        e.OrderId == orderId && 
                        e.CustomerId == customerId && 
                        e.PreviousStatus == "Pending" && 
                        e.NewStatus == newStatus && 
                        e.ChangedById == updatedById && 
                        e.Notes == notes),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CancelOrderAsync_ShouldCancelOrderAndPublishEvent()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                CustomerId = customerId,
                Status = "Pending",
                OrderDate = DateTime.UtcNow,
                TotalAmount = 100
            };
            
            _orderRepositoryMock
                .Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync(order);
            
            var reason = "Customer requested cancellation";
            var cancelledById = Guid.NewGuid();

            // Act
            await _orderService.CancelOrderAsync(orderId, reason, cancelledById);

            // Assert
            Assert.Equal("Cancelled", order.Status);
            
            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(),
                Times.Once);
            
            _eventBusMock.Verify(
                eb => eb.PublishAsync(
                    It.Is<OrderCancelledEvent>(e => 
                        e.OrderId == orderId && 
                        e.CustomerId == customerId && 
                        e.Reason == reason && 
                        e.CancelledById == cancelledById),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CancelOrderAsync_WithShippedOrder_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                Status = "Shipped",
                OrderDate = DateTime.UtcNow
            };
            
            _orderRepositoryMock
                .Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync(order);
            
            var reason = "Customer requested cancellation";

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _orderService.CancelOrderAsync(orderId, reason));
            
            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(),
                Times.Never);
            
            _eventBusMock.Verify(
                eb => eb.PublishAsync(
                    It.IsAny<OrderCancelledEvent>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}