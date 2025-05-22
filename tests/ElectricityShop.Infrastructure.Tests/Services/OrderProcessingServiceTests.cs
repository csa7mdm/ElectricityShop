using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Application.Features.Orders.Models;
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Events;
using ElectricityShop.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ElectricityShop.Infrastructure.Tests.Services
{
    public class OrderProcessingServiceTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly Mock<IPaymentService> _paymentServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<RetryPolicyService> _retryPolicyServiceMock;
        private readonly Mock<ILogger<OrderProcessingService>> _loggerMock;
        private readonly OrderProcessingService _service;
        
        public OrderProcessingServiceTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _paymentServiceMock = new Mock<IPaymentService>();
            _emailServiceMock = new Mock<IEmailService>();
            _mediatorMock = new Mock<IMediator>();
            _retryPolicyServiceMock = new Mock<RetryPolicyService>();
            _loggerMock = new Mock<ILogger<OrderProcessingService>>();
            
            _service = new OrderProcessingService(
                _contextMock.Object,
                _paymentServiceMock.Object,
                _emailServiceMock.Object,
                _mediatorMock.Object,
                _retryPolicyServiceMock.Object,
                _loggerMock.Object);
        }
        
        [Fact]
        public async Task ProcessPaymentAsync_SuccessfulPayment_UpdatesOrderStatus()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                OrderNumber = "ORD-123",
                Status = OrderStatus.PaymentPending,
                TotalAmount = 99.99m,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 99.99m, TotalPrice = 99.99m }
                }
            };
            
            var paymentMethodId = Guid.NewGuid();
            order.PaymentMethod = paymentMethodId.ToString();
            
            var ordersMock = new List<Order> { order }.AsQueryable();
            var mockOrdersDbSet = CreateMockDbSet(ordersMock);
            
            _contextMock.Setup(c => c.Orders)
                .Returns(mockOrdersDbSet.Object);
                
            // Add Include and FirstOrDefaultAsync behavior
            _contextMock.Setup(c => c.Orders.Include(It.IsAny<string>()))
                .Returns(mockOrdersDbSet.Object);
                
            mockOrdersDbSet.Setup(d => d.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);
                
            // Setup payment service
            _paymentServiceMock.Setup(p => p.ProcessPaymentAsync(
                orderId, 99.99m, paymentMethodId))
                .ReturnsAsync(new PaymentResult
                {
                    Success = true,
                    TransactionId = "tx_123456"
                });
                
            // Act
            await _service.ProcessPaymentAsync(orderId);
            
            // Assert
            Assert.Equal(OrderStatus.PaymentProcessed, order.Status);
            Assert.Equal("tx_123456", order.PaymentTransactionId);
            
            // Verify SaveChangesAsync was called
            _contextMock.Verify(c => c.SaveChangesAsync(), Times.Once);
            
            // Verify event was published
            _mediatorMock.Verify(m => m.Publish(
                It.Is<OrderPaymentProcessedEvent>(e =>
                    e.OrderId == orderId &&
                    e.OrderNumber == "ORD-123" &&
                    e.PaymentSuccess == true &&
                    e.TransactionId == "tx_123456"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async Task ProcessPaymentAsync_FailedPayment_UpdatesOrderStatusToFailed()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                OrderNumber = "ORD-123",
                Status = OrderStatus.PaymentPending,
                TotalAmount = 99.99m,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 99.99m, TotalPrice = 99.99m }
                }
            };
            
            var paymentMethodId = Guid.NewGuid();
            order.PaymentMethod = paymentMethodId.ToString();
            
            var ordersMock = new List<Order> { order }.AsQueryable();
            var mockOrdersDbSet = CreateMockDbSet(ordersMock);
            
            _contextMock.Setup(c => c.Orders)
                .Returns(mockOrdersDbSet.Object);
                
            // Add Include and FirstOrDefaultAsync behavior
            _contextMock.Setup(c => c.Orders.Include(It.IsAny<string>()))
                .Returns(mockOrdersDbSet.Object);
                
            mockOrdersDbSet.Setup(d => d.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);
                
            // Setup payment service to return failed result
            _paymentServiceMock.Setup(p => p.ProcessPaymentAsync(
                orderId, 99.99m, paymentMethodId))
                .ReturnsAsync(new PaymentResult
                {
                    Success = false,
                    ErrorMessage = "Insufficient funds"
                });
                
            // Act & Assert
            // The method should throw PaymentFailedException to prevent continuation jobs
            var exception = await Assert.ThrowsAsync<PaymentFailedException>(
                () => _service.ProcessPaymentAsync(orderId));
                
            // Verify order status was updated
            Assert.Equal(OrderStatus.Failed, order.Status);
            Assert.Contains("Insufficient funds", order.StatusNotes);
            
            // Verify SaveChangesAsync was called
            _contextMock.Verify(c => c.SaveChangesAsync(), Times.Once);
            
            // Verify event was published
            _mediatorMock.Verify(m => m.Publish(
                It.Is<OrderPaymentProcessedEvent>(e =>
                    e.OrderId == orderId &&
                    e.OrderNumber == "ORD-123" &&
                    e.PaymentSuccess == false &&
                    e.ErrorMessage == "Insufficient funds"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async Task UpdateInventoryAsync_SuccessfulPayment_UpdatesProductStock()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            
            var order = new Order
            {
                Id = orderId,
                OrderNumber = "ORD-123",
                Status = OrderStatus.PaymentProcessed,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = productId, Quantity = 3 }
                }
            };
            
            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                StockQuantity = 10
            };
            
            var ordersMock = new List<Order> { order }.AsQueryable();
            var mockOrdersDbSet = CreateMockDbSet(ordersMock);
            
            _contextMock.Setup(c => c.Orders)
                .Returns(mockOrdersDbSet.Object);
                
            // Add Include and FirstOrDefaultAsync behavior
            _contextMock.Setup(c => c.Orders.Include(It.IsAny<string>()))
                .Returns(mockOrdersDbSet.Object);
                
            mockOrdersDbSet.Setup(d => d.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);
                
            // Setup Products FindAsync
            _contextMock.Setup(c => c.Products.FindAsync(productId))
                .ReturnsAsync(product);
                
            // Act
            await _service.UpdateInventoryAsync(orderId);
            
            // Assert
            Assert.Equal(7, product.StockQuantity); // 10 - 3 = 7
            
            // Verify SaveChangesAsync was called
            _contextMock.Verify(c => c.SaveChangesAsync(), Times.Once);
        }
        
        [Fact]
        public async Task FinalizeOrderAsync_OrderPaid_UpdatesStatusToFulfilled()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                OrderNumber = "ORD-123",
                Status = OrderStatus.PaymentProcessed
            };
            
            _contextMock.Setup(c => c.Orders.FindAsync(orderId))
                .ReturnsAsync(order);
                
            // Act
            await _service.FinalizeOrderAsync(orderId);
            
            // Assert
            Assert.Equal(OrderStatus.Fulfilled, order.Status);
            Assert.NotNull(order.FulfilledAt);
            
            // Verify SaveChangesAsync was called
            _contextMock.Verify(c => c.SaveChangesAsync(), Times.Once);
            
            // Verify event was published
            _mediatorMock.Verify(m => m.Publish(
                It.Is<OrderFulfilledEvent>(e =>
                    e.OrderId == orderId &&
                    e.OrderNumber == "ORD-123"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async Task SendOrderConfirmationEmailAsync_SuccessfulEmail_LogsSuccess()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            
            var order = new Order
            {
                Id = orderId,
                OrderNumber = "ORD-123",
                Status = OrderStatus.PaymentProcessed,
                UserId = userId,
                User = new User { Id = userId, Email = "customer@example.com", UserName = "Customer" },
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductName = "Test Product",
                        Quantity = 2,
                        UnitPrice = 9.99m,
                        TotalPrice = 19.98m
                    }
                },
                TotalAmount = 19.98m
            };
            
            var ordersMock = new List<Order> { order }.AsQueryable();
            var mockOrdersDbSet = CreateMockDbSet(ordersMock);
            
            _contextMock.Setup(c => c.Orders)
                .Returns(mockOrdersDbSet.Object);
                
            // Add Include and FirstOrDefaultAsync behavior for nested includes
            _contextMock.Setup(c => c.Orders.Include(It.IsAny<string>()))
                .Returns(mockOrdersDbSet.Object);
                
            mockOrdersDbSet.Setup(d => d.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);
                
            // Setup email service
            _emailServiceMock.Setup(e => e.SendOrderConfirmationAsync(
                "customer@example.com",
                "Customer",
                "ORD-123",
                It.IsAny<List<OrderItemEmailModel>>(),
                19.98m))
                .ReturnsAsync(new EmailResult { Sent = true });
                
            // Act
            await _service.SendOrderConfirmationEmailAsync(orderId);
            
            // Assert - no exception means success
            // Verify email service was called with correct parameters
            _emailServiceMock.Verify(e => e.SendOrderConfirmationAsync(
                "customer@example.com",
                "Customer",
                "ORD-123",
                It.Is<List<OrderItemEmailModel>>(items => items.Count == 1),
                19.98m),
                Times.Once);
        }
        
        [Fact]
        public async Task SendOrderConfirmationEmailAsync_FailedEmail_LogsErrorButDoesntThrow()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            
            var order = new Order
            {
                Id = orderId,
                OrderNumber = "ORD-123",
                Status = OrderStatus.PaymentProcessed,
                UserId = userId,
                User = new User { Id = userId, Email = "customer@example.com", UserName = "Customer" },
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductName = "Test Product",
                        Quantity = 2,
                        UnitPrice = 9.99m,
                        TotalPrice = 19.98m
                    }
                },
                TotalAmount = 19.98m
            };
            
            var ordersMock = new List<Order> { order }.AsQueryable();
            var mockOrdersDbSet = CreateMockDbSet(ordersMock);
            
            _contextMock.Setup(c => c.Orders)
                .Returns(mockOrdersDbSet.Object);
                
            // Add Include and FirstOrDefaultAsync behavior for nested includes
            _contextMock.Setup(c => c.Orders.Include(It.IsAny<string>()))
                .Returns(mockOrdersDbSet.Object);
                
            mockOrdersDbSet.Setup(d => d.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);
                
            // Setup email service to fail
            _emailServiceMock.Setup(e => e.SendOrderConfirmationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<List<OrderItemEmailModel>>(),
                It.IsAny<decimal>()))
                .ReturnsAsync(new EmailResult
                {
                    Sent = false,
                    ErrorMessage = "SMTP server unavailable"
                });
                
            // Act - should not throw even though email fails
            await _service.SendOrderConfirmationEmailAsync(orderId);
            
            // Verify error was logged
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to send confirmation email")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
        
        // Helper methods
        private Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }
    }
}