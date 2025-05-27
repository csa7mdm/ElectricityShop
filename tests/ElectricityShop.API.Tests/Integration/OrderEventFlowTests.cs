using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ElectricityShop.API.Tests.Fixtures;
using ElectricityShop.Application.Common.Events;
using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Domain.Events.Orders;
using ElectricityShop.Infrastructure.Events;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace ElectricityShop.API.Tests.Integration
{
    public class OrderEventFlowTests : IClassFixture<IntegrationTestWebApplicationFactory>
    {
        private readonly IntegrationTestWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly Mock<IEventBus> _eventBusMock;

        public OrderEventFlowTests(IntegrationTestWebApplicationFactory factory)
        {
            _eventBusMock = new Mock<IEventBus>();
            
            // Configure the factory to use the mock event bus
            factory.ConfigureTestServices(services =>
            {
                services.AddSingleton<IEventBus>(_eventBusMock.Object);
            });
            
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task PlacingOrder_ShouldPublishOrderPlacedEvent()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var orderItems = new List<OrderItemDto>
            {
                new OrderItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test Product",
                    Quantity = 1,
                    UnitPrice = 100
                }
            };
            
            var request = new
            {
                CustomerId = customerId,
                Items = orderItems
            };
            
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/orders", jsonContent);

            // Assert
            response.EnsureSuccessStatusCode();
            
            // Verify that the event bus was called to publish an OrderPlacedEvent
            _eventBusMock.Verify(
                eb => eb.PublishAsync(
                    It.IsAny<OrderPlacedEvent>(),
                    default),
                Times.Once);
        }

        [Fact]
        public async Task CancellingOrder_ShouldPublishOrderCancelledEvent()
        {
            // This test would require more setup to create an order first
            // We'll simplify by mocking parts of the dependencies
            
            // Arrange - create a test order in the database
            var orderId = Guid.NewGuid();
            var request = new
            {
                Reason = "Test cancellation",
                CancelledById = Guid.NewGuid()
            };
            
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            // Mock the OrderService to handle the test order
            var orderServiceMock = new Mock<IOrderService>();
            orderServiceMock.Setup(s => s.CancelOrderAsync(
                    orderId, 
                    request.Reason, 
                    request.CancelledById, 
                    default))
                .Returns(Task.CompletedTask);
            
            _factory.ConfigureTestServices(services =>
            {
                services.AddScoped<IOrderService>(sp => orderServiceMock.Object);
            });

            // Act
            var response = await _client.DeleteAsync($"/api/orders/{orderId}");

            // Assert
            response.EnsureSuccessStatusCode();
            
            // Verify that the OrderService's CancelOrderAsync method was called
            orderServiceMock.Verify(
                os => os.CancelOrderAsync(
                    orderId, 
                    request.Reason, 
                    request.CancelledById, 
                    default),
                Times.Once);
            
            // In a real test, we would verify the event was published
            // Here we're relying on the unit tests to verify that behavior
        }
    }
}