using ElectricityShop.API.Controllers;
using ElectricityShop.Application.Features.Products.Commands;
using ElectricityShop.Application.Features.Products.Queries;
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
    public class ProductsControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new ProductsController(_mockMediator.Object);
        }

        [Fact]
        public async Task GetProducts_ReturnsProducts()
        {
            // Arrange
            var paginationParams = new ElectricityShop.API.Models.PaginationParams
            {
                PageNumber = 1,
                PageSize = 10
            };
            
            var query = new GetProductsQuery();
            var products = new List<ElectricityShop.Application.Features.Products.Queries.ProductDto>
            {
                new ElectricityShop.Application.Features.Products.Queries.ProductDto { Id = Guid.NewGuid(), Name = "Product 1" },
                new ElectricityShop.Application.Features.Products.Queries.ProductDto { Id = Guid.NewGuid(), Name = "Product 2" }
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductsQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(products));

            // Act
            var result = await _controller.GetProducts(paginationParams, query);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ElectricityShop.API.Models.PagedResponse<ElectricityShop.Application.Features.Products.Queries.ProductDto>>>(result);
            var pagedResponse = actionResult.Value as ElectricityShop.API.Models.PagedResponse<ElectricityShop.Application.Features.Products.Queries.ProductDto>;
            Assert.NotNull(pagedResponse);
            Assert.Equal(2, pagedResponse.Items.Count);
            
            _mockMediator.Verify(m => m.Send(It.IsAny<GetProductsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetProduct_ReturnsProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new ElectricityShop.Application.Features.Products.Queries.ProductDto { Id = productId, Name = "Test Product" };
            
            _mockMediator.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(product));

            // Act
            var result = await _controller.GetProduct(productId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ElectricityShop.Application.Features.Products.Queries.ProductDto>>(result);
            var returnValue = Assert.IsType<ElectricityShop.Application.Features.Products.Queries.ProductDto>(actionResult.Value);
            Assert.Equal(productId, returnValue.Id);
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedAtAction()
        {
            // Arrange
            var command = new ElectricityShop.Application.Features.Products.Commands.CreateProductCommand
            {
                Name = "New Product",
                Description = "Product description",
                Price = 99.99m,
                StockQuantity = 10,
                CategoryId = Guid.NewGuid()
            };
            var productId = Guid.NewGuid();

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(productId));

            // Setup controller context with Admin role for authorization
            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Role, "Admin")
            }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.CreateProduct(command);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Guid>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetProduct", createdAtActionResult.ActionName);
            Assert.Equal(productId, createdAtActionResult.Value);
            Assert.Equal(productId, createdAtActionResult.RouteValues["id"]);
            
            _mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new UpdateProductCommand
            {
                Id = productId,
                Name = "Updated Product",
                Description = "Updated description",
                Price = 109.99m,
                StockQuantity = 5,
                CategoryId = Guid.NewGuid(),
                IsActive = true
            };

            _mockMediator.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            // Setup controller context with Admin role for authorization
            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Role, "Admin")
            }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.UpdateProduct(productId, command);

            // Assert
            var statusCodeResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, statusCodeResult.StatusCode);
            
            _mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            // Setup controller context with Admin role for authorization
            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Role, "Admin")
            }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            var statusCodeResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, statusCodeResult.StatusCode);
            
            _mockMediator.Verify(m => m.Send(It.Is<DeleteProductCommand>(cmd => cmd.Id == productId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}