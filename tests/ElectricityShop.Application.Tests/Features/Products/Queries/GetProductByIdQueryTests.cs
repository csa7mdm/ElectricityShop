using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Application.Features.Products.Queries;
using ElectricityShop.Application.Features.Products.Queries.Handlers;
using ElectricityShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ElectricityShop.Application.Tests.Features.Products.Queries
{
    public class GetProductByIdQueryTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly GetProductByIdQueryHandler _handler;
        
        public GetProductByIdQueryTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _cacheServiceMock = new Mock<ICacheService>();
            _handler = new GetProductByIdQueryHandler(_contextMock.Object, _cacheServiceMock.Object);
        }
        
        [Fact]
        public async Task Handle_CacheHit_ReturnsFromCache()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var cacheKey = $"product:{productId}";
            
            var cachedProduct = new ProductDto
            {
                Id = productId,
                Name = "Cached Product",
                Description = "From Cache",
                Price = 9.99m
            };
            
            _cacheServiceMock.Setup(x => x.GetAsync<ProductDto>(cacheKey))
                .ReturnsAsync(cachedProduct);
                
            var query = new GetProductByIdQuery { Id = productId };
            
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal("Cached Product", result.Name);
            
            // Verify cache was checked
            _cacheServiceMock.Verify(x => x.GetAsync<ProductDto>(cacheKey), Times.Once);
            
            // Verify database was not queried
            _contextMock.Verify(x => x.Products, Times.Never);
        }
        
        [Fact]
        public async Task Handle_CacheMiss_QueriesDatabaseAndCaches()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var cacheKey = $"product:{productId}";
            
            // Setup cache miss
            _cacheServiceMock.Setup(x => x.GetAsync<ProductDto>(cacheKey))
                .ReturnsAsync((ProductDto)null);
                
            // Setup database result
            var product = new Product
            {
                Id = productId,
                Name = "Database Product",
                Description = "From Database",
                Price = 19.99m,
                CategoryId = categoryId
            };
            
            var category = new Category
            {
                Id = categoryId,
                Name = "Test Category"
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
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal("Database Product", result.Name);
            
            // Verify cache was checked
            _cacheServiceMock.Verify(x => x.GetAsync<ProductDto>(cacheKey), Times.Once);
            
            // Verify result was cached
            _cacheServiceMock.Verify(x => x.SetAsync(
                cacheKey, 
                It.Is<ProductDto>(p => p.Id == productId), 
                It.IsAny<TimeSpan?>()), 
                Times.Once);
        }
        
        [Fact]
        public async Task Handle_ProductNotFound_ReturnsNull()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var cacheKey = $"product:{productId}";
            
            // Setup cache miss
            _cacheServiceMock.Setup(x => x.GetAsync<ProductDto>(cacheKey))
                .ReturnsAsync((ProductDto)null);
                
            // Empty database result
            var products = new List<Product>().AsQueryable();
            var categories = new List<Category>().AsQueryable();
            
            var productsMock = MockDbSet(products);
            var categoriesMock = MockDbSet(categories);
            
            _contextMock.Setup(ctx => ctx.Products).Returns(productsMock.Object);
            _contextMock.Setup(ctx => ctx.Categories).Returns(categoriesMock.Object);
            
            var query = new GetProductByIdQuery { Id = productId };
            
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            
            // Assert
            Assert.Null(result);
            
            // Verify cache was checked
            _cacheServiceMock.Verify(x => x.GetAsync<ProductDto>(cacheKey), Times.Once);
            
            // Verify result was not cached (because it's null)
            _cacheServiceMock.Verify(x => x.SetAsync(
                It.IsAny<string>(), 
                It.IsAny<ProductDto>(), 
                It.IsAny<TimeSpan?>()), 
                Times.Never);
        }
        
        // Helper method for mocking DbSet
        private static Mock<DbSet<T>> MockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mock = new Mock<DbSet<T>>();
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            
            return mock;
        }
    }
}