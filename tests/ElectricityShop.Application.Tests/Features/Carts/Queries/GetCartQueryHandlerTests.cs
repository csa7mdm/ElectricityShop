using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Features.Carts.Queries;
using ElectricityShop.Application.Features.Carts.Queries.Handlers;
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ElectricityShop.Application.Tests.Features.Carts.Queries
{
    public class GetCartQueryHandlerTests
    {
        private readonly Mock<IRepository<Cart>> _mockCartRepository;
        private readonly Mock<IRepository<Product>> _mockProductRepository; // For fallback, if used
        private readonly Mock<ILogger<GetCartQueryHandler>> _mockLogger;
        private readonly GetCartQueryHandler _handler;

        public GetCartQueryHandlerTests()
        {
            _mockCartRepository = new Mock<IRepository<Cart>>();
            _mockProductRepository = new Mock<IRepository<Product>>();
            _mockLogger = new Mock<ILogger<GetCartQueryHandler>>();
            _handler = new GetCartQueryHandler(
                _mockCartRepository.Object,
                _mockProductRepository.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCartDto_WhenCartExistsForUser()
        {
            // Arrange
            var testUserId = Guid.NewGuid();
            var testCartId = Guid.NewGuid();
            var testProductId = Guid.NewGuid();
            var query = new GetCartQuery { UserId = testUserId };

            var product = new Product 
            { 
                Id = testProductId, 
                Name = "Test Product", 
                Price = 10.0m, // Price on Product entity, UnitPrice on CartItem is what's used by DTO
                Images = new List<ProductImage> { new ProductImage { ImageUrl = "test.jpg", IsMain = true } }
            };

            var cartEntity = new Cart
            {
                Id = testCartId,
                UserId = testUserId,
                Items = new List<CartItem>
                {
                    new CartItem 
                    { 
                        ProductId = testProductId, 
                        Quantity = 2, 
                        UnitPrice = 10.0m, // This is the price at the time of adding to cart
                        Product = product 
                    }
                }
            };

            _mockCartRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                .ReturnsAsync((Expression<Func<Cart, bool>> predicate) => 
                {
                    // Simulate the predicate execution for more accurate mocking
                    // This basic simulation assumes the predicate is c => c.UserId == testUserId
                    // A more robust mock might require compiling and invoking the predicate if it's complex.
                    // For this test, we directly check if the cart's UserId matches.
                    return cartEntity.UserId == testUserId ? cartEntity : null;
                });
            
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testCartId, result.Id);
            Assert.Equal(testUserId, result.UserId);
            Assert.Single(result.Items);
            
            var itemDto = result.Items.First();
            Assert.Equal(testProductId, itemDto.ProductId);
            Assert.Equal("Test Product", itemDto.ProductName);
            Assert.Equal(10.0m, itemDto.UnitPrice); // Verifying against CartItem.UnitPrice
            Assert.Equal(2, itemDto.Quantity);
            Assert.Equal(20.0m, itemDto.TotalPrice); // Assuming DTO calculates this: 10.0m * 2
            Assert.Equal("test.jpg", itemDto.ProductImageUrl);

            _mockCartRepository.Verify(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenCartDoesNotExistForUser()
        {
            // Arrange
            var testUserId = Guid.NewGuid();
            var query = new GetCartQuery { UserId = testUserId };

            _mockCartRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                .ReturnsAsync((Cart?)null); // Simulate cart not found. Explicit cast to Cart? or (Cart)null
            
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _mockCartRepository.Verify(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldUseFallbackToFetchProduct_WhenProductNotIncludedInCartItem()
        {
            // Arrange
            var testUserId = Guid.NewGuid();
            var testCartId = Guid.NewGuid();
            var testProductId = Guid.NewGuid();
            var query = new GetCartQuery { UserId = testUserId };

            var product = new Product 
            { 
                Id = testProductId, 
                Name = "Fallback Product", 
                Price = 15.0m,
                Images = new List<ProductImage> { new ProductImage { ImageUrl = "fallback.jpg", IsMain = true } }
            };
            
            // Simulate CartItem where Product is initially null (not loaded)
            var cartEntity = new Cart
            {
                Id = testCartId,
                UserId = testUserId,
                Items = new List<CartItem>
                {
                    new CartItem 
                    { 
                        ProductId = testProductId, 
                        Quantity = 1, 
                        UnitPrice = 15.0m, 
                        Product = null // Product not included initially
                    }
                }
            };

            _mockCartRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                .ReturnsAsync((Expression<Func<Cart, bool>> predicate) => 
                {
                     return cartEntity.UserId == testUserId ? cartEntity : null;
                });

            _mockProductRepository.Setup(r => r.GetByIdAsync(testProductId))
                .ReturnsAsync(product); // Setup fallback product fetch

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            var itemDto = result.Items.First();
            Assert.Equal("Fallback Product", itemDto.ProductName);
            Assert.Equal(15.0m, itemDto.UnitPrice);
            Assert.Equal("fallback.jpg", itemDto.ProductImageUrl);

            _mockCartRepository.Verify(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>()), Times.Once);
            _mockProductRepository.Verify(r => r.GetByIdAsync(testProductId), Times.Once); // Verify fallback was called
        }
    }
}
