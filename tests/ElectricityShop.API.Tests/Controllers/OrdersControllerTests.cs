using ElectricityShop.API.Controllers;
using ElectricityShop.Application.Features.Orders.Commands;
using ElectricityShop.Application.Features.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Language.Flow;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http;

namespace ElectricityShop.API.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly OrdersController _controller;
        private readonly Guid _userId;

        public OrdersControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new OrdersController(_mockMediator.Object);
            _userId = Guid.NewGuid();

            // Setup controller context with claims identity for authorization
            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim("id", _userId.ToString()),
                new Claim(ClaimTypes.Name, "testuser@example.com"),
                new Claim(ClaimTypes.Role, "Customer")
            }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetOrders_ReturnsOrders()
        {
            // Arrange
            var query = new GetOrdersQuery();
            var orders = new List<ElectricityShop.Application.Features.Orders.Queries.OrderDto>
            {
                new ElectricityShop.Application.Features.Orders.Queries.OrderDto { Id = Guid.NewGuid(), OrderNumber = "ORD-001" },
                new ElectricityShop.Application.Features.Orders.Queries.OrderDto { Id = Guid.NewGuid(), OrderNumber = "ORD-002" }
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetOrdersQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(orders));

            // Act
            var result = await _controller.GetOrders(query);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<ElectricityShop.Application.Features.Orders.Queries.OrderDto>>>(result);
            var returnValue = Assert.IsType<List<ElectricityShop.Application.Features.Orders.Queries.OrderDto>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
            
            // Verify userId is set from claims
            _mockMediator.Verify(m => m.Send(It.Is<GetOrdersQuery>(q => q.UserId == _userId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetOrder_ReturnsNotImplemented()
        {
            // Act
            var result = await _controller.GetOrder(Guid.NewGuid());

            // Assert
            var actionResult = Assert.IsType<ActionResult<ElectricityShop.Application.Features.Orders.Queries.OrderDto>>(result);
            var statusCodeResult = Assert.IsType<StatusCodeResult>(actionResult.Result);
            Assert.Equal(501, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task CreateOrder_ReturnsCreatedAtAction()
        {
            // Arrange
            var command = new CreateOrderCommand();
            var orderId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(orderId));

            // Act
            var result = await _controller.CreateOrder(command);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Guid>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetOrder", createdAtActionResult.ActionName);
            Assert.Equal(orderId, createdAtActionResult.Value);
            Assert.Equal(orderId, createdAtActionResult.RouteValues["id"]);
            
            // Verify userId is set from claims
            _mockMediator.Verify(m => m.Send(It.Is<CreateOrderCommand>(c => c.UserId == _userId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelOrder_ReturnsNotImplemented()
        {
            // Act
            var result = await _controller.CancelOrder(Guid.NewGuid());

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(501, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task ProcessPayment_ReturnsNotImplemented()
        {
            // Arrange
            var request = new ProcessPaymentRequest
            {
                CardNumber = "4111111111111111",
                CardHolderName = "Test User",
                ExpiryMonth = "12",
                ExpiryYear = "2025",
                CVV = "123",
                BillingAddress = new BillingAddressDto
                {
                    AddressLine1 = "123 Main St",
                    City = "Anytown",
                    State = "State",
                    Country = "Country",
                    ZipCode = "12345"
                }
            };

            // Act
            var result = await _controller.ProcessPayment(Guid.NewGuid(), request);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(501, statusCodeResult.StatusCode);
        }
    }
}