using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Application.Features.Products.Commands;
using ElectricityShop.Application.Features.Products.Commands.Handlers;
using ElectricityShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ElectricityShop.Application.Tests.Features.Products.Commands
{
    public class UpdateProductCommandTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly Mock<ICacheInvalidationService> _cacheInvalidationMock;
        private readonly UpdateProductCommandHandler _handler;
        
        public UpdateProductCommandTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _cacheInvalidationMock = new Mock<ICacheInvalidationService>();
            _handler = new UpdateProductCommandHandler(_contextMock.Object, _cacheInvalidationMock.Object);
        }
        
        [Fact]
        public async Task Handle_ExistingProduct_UpdatesAndInvalidatesCache()
        {
            // Arrange
            var existingProductId = Guid.NewGuid();
            var oldCategoryId = Guid.NewGuid();
            var newCategoryId = Guid.NewGuid();
            
            var product = new Product
            {
                Id = existingProductId,
                Name = "Old Name",
                Description = "Old Description",
                Price = 9.99m,
                StockQuantity = 10,
                CategoryId = oldCategoryId,
                IsActive = true
            };
            
            var products = new List<Product> { product }.AsQueryable();
            var dbSetMock = MockDbSet(products);
            
            _contextMock.Setup(ctx => ctx.Products).Returns(dbSetMock.Object);
            
            var command = new UpdateProductCommand
            {
                Id = existingProductId,
                Name = "New Name",
                Description = "New Description",
                Price = 19.99m,
                StockQuantity = 20,
                CategoryId = newCategoryId,
                IsActive = true
            };
            
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            
            // Assert
            Assert.True(result);
            
            // Verify product was updated
            Assert.Equal("New Name", product.Name);
            Assert.Equal("New Description", product.Description);
            Assert.Equal(19.99m, product.Price);
            Assert.Equal(20, product.StockQuantity);
            Assert.Equal(newCategoryId, product.CategoryId);
            
            // Verify cache invalidation
            _cacheInvalidationMock.Verify(x => x.InvalidateProductCacheAsync((int)existingProductId), Times.Once);
            _cacheInvalidationMock.Verify(x => x.InvalidateProductsByCategoryAsync((int)oldCategoryId), Times.Once);
            _cacheInvalidationMock.Verify(x => x.InvalidateProductsByCategoryAsync((int)newCategoryId), Times.Once);
            
            // Verify SaveChangesAsync was called
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task Handle_NonExistingProduct_ReturnsFalse()
        {
            // Arrange
            var products = new List<Product>().AsQueryable();
            var dbSetMock = MockDbSet(products);
            
            _contextMock.Setup(ctx => ctx.Products).Returns(dbSetMock.Object);
            
            var command = new UpdateProductCommand
            {
                Id = Guid.NewGuid(),
                Name = "New Name",
                Description = "New Description",
                Price = 19.99m,
                StockQuantity = 20,
                CategoryId = Guid.NewGuid(),
                IsActive = true
            };
            
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            
            // Assert
            Assert.False(result);
            
            // Verify cache invalidation was not called
            _cacheInvalidationMock.Verify(x => x.InvalidateProductCacheAsync(It.IsAny<int>()), Times.Never);
            _cacheInvalidationMock.Verify(x => x.InvalidateProductsByCategoryAsync(It.IsAny<int>()), Times.Never);
            
            // Verify SaveChangesAsync was not called
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
        
        [Fact]
        public async Task Handle_SameCategoryId_OnlyInvalidatesProductCache()
        {
            // Arrange
            var existingProductId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            
            var product = new Product
            {
                Id = existingProductId,
                Name = "Old Name",
                Description = "Old Description",
                Price = 9.99m,
                StockQuantity = 10,
                CategoryId = categoryId,
                IsActive = true
            };
            
            var products = new List<Product> { product }.AsQueryable();
            var dbSetMock = MockDbSet(products);
            
            _contextMock.Setup(ctx => ctx.Products).Returns(dbSetMock.Object);
            
            var command = new UpdateProductCommand
            {
                Id = existingProductId,
                Name = "New Name",
                Description = "New Description",
                Price = 19.99m,
                StockQuantity = 20,
                CategoryId = categoryId, // Same category ID
                IsActive = true
            };
            
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            
            // Assert
            Assert.True(result);
            
            // Verify product was updated
            Assert.Equal("New Name", product.Name);
            
            // Verify only product cache was invalidated
            _cacheInvalidationMock.Verify(x => x.InvalidateProductCacheAsync((int)existingProductId), Times.Once);
            _cacheInvalidationMock.Verify(x => x.InvalidateProductsByCategoryAsync(It.IsAny<int>()), Times.Never);
        }
        
        // Helper method for mocking DbSet
        private static Mock<DbSet<T>> MockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mock = new Mock<DbSet<T>>();
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mock.Setup(d => d.FindAsync(It.IsAny<object[]>())).Returns(ValueTask.FromResult(data.FirstOrDefault()));
            
            return mock;
        }
    }
}