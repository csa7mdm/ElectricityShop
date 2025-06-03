using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Application.Features.Orders.Commands;
using ElectricityShop.Application.Features.Orders.Commands.Handlers;
using ElectricityShop.Application.Features.Orders.Models;
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OrderStatus = ElectricityShop.Domain.Enums.OrderStatus;
using Xunit;

namespace ElectricityShop.Application.Tests.Features.Orders.Commands.Handlers
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly Mock<IBackgroundJobService> _backgroundJobsMock;
        private readonly Mock<IOrderProcessingService> _orderProcessingMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<CreateOrderCommandHandler>> _loggerMock;
        private readonly CreateOrderCommandHandler _handler;
        private readonly DbContextOptions<TestDbContext> _dbContextOptions;
        
        public CreateOrderCommandHandlerTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _backgroundJobsMock = new Mock<IBackgroundJobService>();
            _orderProcessingMock = new Mock<IOrderProcessingService>();
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<CreateOrderCommandHandler>>();
            
            _handler = new CreateOrderCommandHandler(
                _contextMock.Object,
                _backgroundJobsMock.Object,
                _orderProcessingMock.Object,
                _mediatorMock.Object,
                _loggerMock.Object);
                
            // Set up in-memory database for testing
            _dbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"ElectricityShopTestDb_{Guid.NewGuid()}")
                .Options;
                
            // Set up mock DbSets
            var products = new List<Product>
            {
                new Product 
                { 
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), 
                    Name = "Test Product", 
                    Description = "Test Description",
                    Price = 9.99m,
                    StockQuantity = 100 
                }
            }.AsQueryable();
            
            var mockProductsDbSet = CreateMockDbSet(products);
            _contextMock.Setup(c => c.Products).Returns(mockProductsDbSet.Object);
            
            // Mock SaveChangesAsync
            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
                
            // Mock background job service
            _backgroundJobsMock.Setup(x => x.Enqueue(It.IsAny<Func<IOrderProcessingService, Task>>()))
                .Returns("job123");
                
            _backgroundJobsMock.Setup(x => x.ContinueJobWith(It.IsAny<string>(), It.IsAny<Func<IOrderProcessingService, Task>>()))
                .Returns(true);
        }
        
        [Fact]
        public async Task Handle_ValidOrder_CreatesOrderAndSchedulesJobs()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                ShippingAddress = new AddressDto
                {
                    Line1 = "123 Main St",
                    City = "New City",
                    State = "NY",
                    PostalCode = "12345",
                    Country = "USA"
                },
                Items = new List<ElectricityShop.Application.Features.Orders.Commands.OrderItemDto>
                {
                    new ElectricityShop.Application.Features.Orders.Commands.OrderItemDto
                    {
                        ProductId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Quantity = 2
                    }
                },
                PaymentMethod = Guid.NewGuid().ToString(),
                ShippingMethod = "Standard"
            };
            
            // Set up the product find method
            var testProduct = new Product
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Test Product",
                Description = "Test Description",
                Price = 9.99m,
                StockQuantity = 100
            };
            
            _contextMock.Setup(c => c.Products.FindAsync(It.Is<object[]>(ids => 
                ids[0].Equals(Guid.Parse("11111111-1111-1111-1111-111111111111"))), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(testProduct);
                
            // Set up Orders collection to capture the added order
            var orders = new List<Order>();
            var mockOrdersDbSet = new Mock<DbSet<Order>>();
            mockOrdersDbSet.Setup(d => d.Add(It.IsAny<Order>()))
                .Callback<Order>(order => orders.Add(order));
            _contextMock.Setup(c => c.Orders).Returns(mockOrdersDbSet.Object);
            
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            
            // Assert
            // Verify order was created with correct properties
            Assert.Single(orders);
            var createdOrder = orders[0];
            Assert.Equal(command.UserId, createdOrder.UserId);
            Assert.Equal(OrderStatus.PaymentPending, createdOrder.Status);
            Assert.Equal(command.ShippingMethod, createdOrder.ShippingMethod);
            
            // Verify items were added correctly
            Assert.Single(createdOrder.Items);
            var orderItem = createdOrder.Items.First();
            Assert.Equal(Guid.Parse("11111111-1111-1111-1111-111111111111"), orderItem.ProductId);
            Assert.Equal(2, orderItem.Quantity);
            Assert.Equal(9.99m, orderItem.UnitPrice);
            Assert.Equal(19.98m, orderItem.UnitPrice * orderItem.Quantity);
            
            // Verify background jobs were enqueued
            _backgroundJobsMock.Verify(
                x => x.Enqueue(It.IsAny<Func<IOrderProcessingService, Task>>()),
                Times.Once);
            
            _backgroundJobsMock.Verify(
                x => x.ContinueJobWith(It.IsAny<string>(), It.IsAny<Func<IOrderProcessingService, Task>>()),
                Times.Exactly(3)); // 3 continuation jobs
                
            // Verify domain event was published
            _mediatorMock.Verify(
                x => x.Publish(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
                
            // Verify result has expected values
            Assert.NotNull(result);
            Assert.Equal(createdOrder.Id, result.OrderId);
            Assert.Equal(createdOrder.OrderNumber, result.OrderNumber);
            Assert.Equal(ElectricityShop.Application.Features.Orders.Models.OrderStatus.PaymentPending, result.Status);
            Assert.Equal("job123", result.TrackingId);
            Assert.NotNull(result.EstimatedCompletion);
        }
        
        [Fact]
        public async Task Handle_InsufficientStock_ThrowsException()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                ShippingAddress = new AddressDto
                {
                    Line1 = "123 Main St",
                    City = "New City",
                    State = "NY",
                    PostalCode = "12345",
                    Country = "USA"
                },
                Items = new List<ElectricityShop.Application.Features.Orders.Commands.OrderItemDto>
                {
                    new ElectricityShop.Application.Features.Orders.Commands.OrderItemDto
                    {
                        ProductId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Quantity = 101 // More than available stock
                    }
                },
                PaymentMethod = Guid.NewGuid().ToString(),
                ShippingMethod = "Standard"
            };
            
            // Set up the product find method with insufficient stock
            _contextMock.Setup(c => c.Products.FindAsync(It.Is<object[]>(ids => 
                ids[0].Equals(Guid.Parse("11111111-1111-1111-1111-111111111111"))), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Product
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Test Product",
                    Description = "Test Description",
                    Price = 9.99m,
                    StockQuantity = 100 // Less than requested
                });
                
            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));
                
            Assert.Contains("Insufficient stock", exception.Message);
        }
        
        [Fact]
        public async Task Handle_ProductNotFound_ThrowsException()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                ShippingAddress = new AddressDto
                {
                    Line1 = "123 Main St",
                    City = "New City",
                    State = "NY",
                    PostalCode = "12345",
                    Country = "USA"
                },
                Items = new List<ElectricityShop.Application.Features.Orders.Commands.OrderItemDto>
                {
                    new ElectricityShop.Application.Features.Orders.Commands.OrderItemDto
                    {
                        ProductId = Guid.Parse("99999999-9999-9999-9999-999999999999"), // Non-existent product
                        Quantity = 2
                    }
                },
                PaymentMethod = Guid.NewGuid().ToString(),
                ShippingMethod = "Standard"
            };
            
            // Set up the product find method to return null
            Product? nullProduct = null;
            _contextMock.Setup(c => c.Products.FindAsync(It.Is<object[]>(ids => 
                ids[0].Equals(Guid.Parse("99999999-9999-9999-9999-999999999999"))), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(nullProduct);
                
            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));
                
            Assert.Contains("Product not found", exception.Message);
        }
        
        [Fact]
        public async Task Handle_DatabaseError_LogsAndRethrows()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                ShippingAddress = new AddressDto { Line1 = "123 Main St" },
                Items = new List<ElectricityShop.Application.Features.Orders.Commands.OrderItemDto>
                {
                    new ElectricityShop.Application.Features.Orders.Commands.OrderItemDto
                    {
                        ProductId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Quantity = 2
                    }
                }
            };
            
            // Set up the product find method
            _contextMock.Setup(c => c.Products.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Product
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Test Product",
                    Price = 9.99m,
                    StockQuantity = 100
                });
                
            // Simulate a database error
            var dbException = new DbUpdateException("Database error");
            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(dbException);
                
            // Act & Assert
            var exception = await Assert.ThrowsAsync<DbUpdateException>(
                () => _handler.Handle(command, CancellationToken.None));
                
            // Verify exception is the same one we threw
            Assert.Same(dbException, exception);
            
            // Verify error was logged
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>(v => v.ToString().Contains("Error creating order")),
                    It.Is<DbUpdateException>(ex => ex == dbException),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
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
        
        // Helper DbContext for in-memory testing
        private class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions options) : base(options) { }
            
            public DbSet<Product> Products { get; set; } = null!;
            public DbSet<Order> Orders { get; set; } = null!;
        }
    }
}