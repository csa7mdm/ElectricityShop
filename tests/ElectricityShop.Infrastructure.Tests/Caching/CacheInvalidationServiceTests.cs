using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Infrastructure.Caching;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace ElectricityShop.Infrastructure.Tests.Caching
{
    public class CacheInvalidationServiceTests
    {
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly CacheInvalidationService _invalidationService;
        
        public CacheInvalidationServiceTests()
        {
            _cacheServiceMock = new Mock<ICacheService>();
            _invalidationService = new CacheInvalidationService(_cacheServiceMock.Object);
        }
        
        [Fact]
        public async Task InvalidateProductCacheAsync_RemovesProductCache_AndAllProductsCache()
        {
            // Arrange
            int productId = 123;
            
            // Act
            await _invalidationService.InvalidateProductCacheAsync(productId);
            
            // Assert
            _cacheServiceMock.Verify(x => x.RemoveAsync($"product:{productId}"), Times.Once);
            _cacheServiceMock.Verify(x => x.RemoveByPrefixAsync("products"), Times.Once);
        }
        
        [Fact]
        public async Task InvalidateAllProductsCacheAsync_RemovesAllProductsCache()
        {
            // Act
            await _invalidationService.InvalidateAllProductsCacheAsync();
            
            // Assert
            _cacheServiceMock.Verify(x => x.RemoveByPrefixAsync("products"), Times.Once);
        }
        
        [Fact]
        public async Task InvalidateProductsByCategoryAsync_RemovesCategoryCache_AndAllProductsCache()
        {
            // Arrange
            int categoryId = 456;
            
            // Act
            await _invalidationService.InvalidateProductsByCategoryAsync(categoryId);
            
            // Assert
            _cacheServiceMock.Verify(x => x.RemoveByPrefixAsync($"products:category:{categoryId}"), Times.Once);
            _cacheServiceMock.Verify(x => x.RemoveByPrefixAsync("products"), Times.Once);
        }
    }
}