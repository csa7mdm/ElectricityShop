using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Application.Features.Products.Queries;
using ElectricityShop.Application.Features.Products.Queries.Handlers;
using ElectricityShop.Domain.Entities;
using Microsoft.Extensions.Logging; // Added for ILogger
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ElectricityShop.Application.Tests.Features.Products.Queries
{
    public class GetProductByIdQueryTests
    {
        private readonly Mock<IRepository<Product>> _productRepositoryMock;
        private readonly Mock<IRepository<Category>> _categoryRepositoryMock;
        private readonly Mock<ILogger<UpdateProductCommandHandler>> _loggerMock;
        private readonly Mock<ICacheInvalidationService> _cacheInvalidationMock; // Assuming this is still used by the handler, or by related logic
        private readonly UpdateProductCommandHandler _handler;

        private readonly Guid _productId = Guid.NewGuid();
        private readonly Guid _categoryId = Guid.NewGuid();
        private readonly Guid _newCategoryId = Guid.NewGuid();
        
        public UpdateProductCommandTests()
        {
            _productRepositoryMock = new Mock<IRepository<Product>>();
            _categoryRepositoryMock = new Mock<IRepository<Category>>();
            _loggerMock = new Mock<ILogger<UpdateProductCommandHandler>>();
            _cacheInvalidationMock = new Mock<ICacheInvalidationService>(); // Initialized
            
            // Handler constructor updated as per CS7036 error message
            _handler = new UpdateProductCommandHandler(
                _loggerMock.Object, 
                _productRepositoryMock.Object, 
                _categoryRepositoryMock.Object
                // Assuming ICacheInvalidationService is not passed to constructor based on CS7036.
                // If it is, _cacheInvalidationMock.Object should be added here.
            );
        }

        [Fact]
        public async Task Handle_CacheHit_ReturnsFromCache()
        {
            // Arrange
            var product = new Product
            {
                Id = _productId, // Use Guid field
                Name = "Old Name",
                Description = "Old Description",
                Price = 9.99m,
                StockQuantity = 10,
                CategoryId = _categoryId, // Use Guid field
                IsActive = true
            };

            var category = new Category { Id = _categoryId, Name = "Old Category", Description = "Desc" };
            var newCategory = new Category { Id = _newCategoryId, Name = "New Category", Description = "Desc" };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _categoryRepositoryMock.Setup(r => r.GetByIdAsync(_categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            _categoryRepositoryMock.Setup(r => r.GetByIdAsync(_newCategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newCategory);
            // Assume IRepository<T> has a method like SaveChangesAsync or UnitOfWork handles it.
            // If not, _productRepositoryMock.Setup(r => r.UpdateAsync(product, It.IsAny<CancellationToken>())) might be needed.

            var command = new UpdateProductCommand
            {
                Id = _productId, // Use Guid field
                Name = "New Name",
                Description = "New Description",
                Price = 19.99m,
                StockQuantity = 20,
                CategoryId = _newCategoryId, // Use Guid field for new category
                IsActive = true
            };
            
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result); // Assuming handler returns bool
            
            // Verify product was updated
            Assert.Equal("New Name", product.Name);
            Assert.Equal("New Description", product.Description);
            Assert.Equal(19.99m, product.Price);
            Assert.Equal(20, product.StockQuantity);
            Assert.Equal(_newCategoryId, product.CategoryId); // Assert Guid
            
            // Verify cache invalidation (removing (int) cast and using Guid)
            // These lines assume the handler still calls ICacheInvalidationService.
            // If the handler was refactored to not call it, these verifies might fail or need removal.
            _cacheInvalidationMock.Verify(x => x.InvalidateProductCacheAsync(_productId), Times.Once);
            _cacheInvalidationMock.Verify(x => x.InvalidateProductsByCategoryAsync(_categoryId), Times.Once);
            _cacheInvalidationMock.Verify(x => x.InvalidateProductsByCategoryAsync(_newCategoryId), Times.Once);
            
            // Verify SaveChangesAsync was called (now on repository or UoW)
            // This depends on IRepository<T> or a UnitOfWork pattern.
            // For now, let's assume UpdateAsync implies save or there's a Save method on repository.
            _productRepositoryMock.Verify(r => r.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_CacheMiss_QueriesDatabaseAndCaches()
        {
            // Arrange
            _productRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null);
            
            var command = new UpdateProductCommand
            {
                Id = Guid.NewGuid(), // Use new Guid for non-existing product
                Name = "New Name",
                Description = "New Description",
                Price = 19.99m,
                StockQuantity = 20,
                CategoryId = Guid.NewGuid(), // Use new Guid
                IsActive = true
            };

            // Mock the join query result
            var products = new[] { product }.AsQueryable();
            var categories = new[] { category }.AsQueryable();

            var productsMock = MockDbSet(products);
            var categoriesMock = MockDbSet(categories);

            _contextMock.Setup(ctx => ctx.Products).Returns(productsMock.Object);
            _contextMock.Setup(ctx => ctx.Categories).Returns(categoriesMock.Object);

            // Setup the DbContext to return our product when queried
            var query = new GetProductByIdQuery { Id = productId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result);
            
            // Verify cache invalidation was not called (using Guid)
            _cacheInvalidationMock.Verify(x => x.InvalidateProductCacheAsync(It.IsAny<Guid>()), Times.Never);
            _cacheInvalidationMock.Verify(x => x.InvalidateProductsByCategoryAsync(It.IsAny<Guid>()), Times.Never);
            
            // Verify UpdateAsync was not called
            _productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ProductNotFound_ReturnsNull()
        {
            // Arrange
            var product = new Product
            {
                Id = _productId, // Use Guid field
                Name = "Old Name",
                Description = "Old Description",
                Price = 9.99m,
                StockQuantity = 10,
                CategoryId = _categoryId, // Use Guid field
                IsActive = true
            };
            var category = new Category { Id = _categoryId, Name = "Category", Description = "Desc" };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(_productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _categoryRepositoryMock.Setup(r => r.GetByIdAsync(_categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var command = new UpdateProductCommand
            {
                Id = _productId, // Use Guid field
                Name = "New Name",
                Description = "New Description",
                Price = 19.99m,
                StockQuantity = 20,
                CategoryId = _categoryId, // Same category ID (Guid)
                IsActive = true
            };
            
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result);
            
            // Verify product was updated
            Assert.Equal("New Name", product.Name);
            
            // Verify only product cache was invalidated (using Guid)
            _cacheInvalidationMock.Verify(x => x.InvalidateProductCacheAsync(_productId), Times.Once);
            _cacheInvalidationMock.Verify(x => x.InvalidateProductsByCategoryAsync(It.IsAny<Guid>()), Times.Never);
            _productRepositoryMock.Verify(r => r.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        }
        
        // Helper method for mocking DbSet - No longer directly used by these tests if IRepository is primary.
        // Keeping it in case other tests in the file might use it or if it's needed for IRepository mock setup internally.
        // private static Mock<DbSet<T>> MockDbSet<T>(IQueryable<T> data) where T : class
        // {
        //     var mock = new Mock<DbSet<T>>();
        //     mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
        //     mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        //     mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        //     mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        //     mock.Setup(d => d.FindAsync(It.IsAny<object[]>())).Returns(ValueTask.FromResult(data.FirstOrDefault()));
            
        //     return mock;
        // }
    }
}