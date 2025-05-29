using ElectricityShop.Application.Features.Products.Queries;
using ElectricityShop.Application.Features.Products.Queries.Handlers;
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Interfaces;
using Microsoft.Extensions.Logging; // For ILogger
using Moq;
using Xunit;

namespace ElectricityShop.Application.Tests.Features.Products.Queries
{
    public class GetProductByIdQueryTests
    {
        private readonly Mock<IRepository<Product>> _productRepositoryMock;
        private readonly Mock<IRepository<Category>> _categoryRepositoryMock;
        private readonly Mock<ILogger<GetProductByIdQueryHandler>> _loggerMock;
        private readonly GetProductByIdQueryHandler _handler;

        private readonly Guid _productId = Guid.NewGuid();
        private readonly Guid _categoryId = Guid.NewGuid();

        public GetProductByIdQueryTests()
        {
            _productRepositoryMock = new Mock<IRepository<Product>>();
            _categoryRepositoryMock = new Mock<IRepository<Category>>();
            _loggerMock = new Mock<ILogger<GetProductByIdQueryHandler>>();

            _handler = new GetProductByIdQueryHandler(
                _loggerMock.Object,
                _productRepositoryMock.Object,
                _categoryRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ProductExists_ReturnsProductDto()
        {
            // Arrange
            var product = new Product
            {
                Id = _productId,
                Name = "Test Product",
                Description = "Test Description",
                Price = 19.99m,
                CategoryId = _categoryId,
                Images = new List<ProductImage>(), // Ensure collections are not null
                Attributes = new List<ProductAttribute>() // Ensure collections are not null
            };

            var category = new Category
            {
                Id = _categoryId,
                Name = "Test Category",
                Description = "Test Category Description" // CS9035 fix
            };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _categoryRepositoryMock.Setup(r => r.GetByIdAsync(_categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category); // CS1061 fix (part 1)

            var query = new GetProductByIdQuery { ProductId = _productId }; // CS0117 fix: Use ProductId

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_productId, result.Id);
            Assert.Equal("Test Product", result.Name);
            Assert.Equal("Test Category", result.CategoryName); // Handler fetches category name

            _productRepositoryMock.Verify(r => r.GetByIdAsync(_productId, It.IsAny<CancellationToken>()), Times.Once);
            _categoryRepositoryMock.Verify(r => r.GetByIdAsync(_categoryId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ProductExists_CategoryPreloaded_ReturnsProductDto()
        {
            // Arrange
            var category = new Category
            {
                Id = _categoryId,
                Name = "Test Category",
                Description = "Test Category Description"
            };
            var product = new Product
            {
                Id = _productId,
                Name = "Test Product",
                Description = "Test Description",
                Price = 19.99m,
                CategoryId = _categoryId,
                Category = category, // Category is preloaded
                Images = new List<ProductImage>(),
                Attributes = new List<ProductAttribute>()
            };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            // No setup for _categoryRepositoryMock.GetByIdAsync needed if Category is preloaded in Product

            var query = new GetProductByIdQuery { ProductId = _productId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_productId, result.Id);
            Assert.Equal("Test Product", result.Name);
            Assert.Equal("Test Category", result.CategoryName);

            _productRepositoryMock.Verify(r => r.GetByIdAsync(_productId, It.IsAny<CancellationToken>()), Times.Once);
            _categoryRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never); // Verify category repo not called
        }

        [Fact]
        public async Task HandleProductNotFoundReturnsNull()
        {
            // Arrange
            var nonExistentProductId = Guid.NewGuid();
            _productRepositoryMock.Setup(r => r.GetByIdAsync(nonExistentProductId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null);

            // No need to setup _categoryRepositoryMock for this case, as product fetch is first
            // _categoryRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Category)null); // CS1061 fix (part 2) - not strictly needed here

            var query = new GetProductByIdQuery { ProductId = nonExistentProductId }; // CS0117 fix

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _productRepositoryMock.Verify(r => r.GetByIdAsync(nonExistentProductId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}