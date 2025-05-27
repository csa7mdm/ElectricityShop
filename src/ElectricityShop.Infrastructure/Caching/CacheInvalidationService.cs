using ElectricityShop.Application.Common.Interfaces;
using System.Threading.Tasks;

namespace ElectricityShop.Infrastructure.Caching
{
    /// <summary>
    /// Service responsible for invalidating cache based on business rules.
    /// </summary>
    public class CacheInvalidationService : ICacheInvalidationService
    {
        private readonly ICacheService _cacheService;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheInvalidationService"/> class.
        /// </summary>
        /// <param name="cacheService">Cache service.</param>
        public CacheInvalidationService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }
        
        /// <inheritdoc />
        public async Task InvalidateProductCacheAsync(int productId)
        {
            // Remove specific product cache
            await _cacheService.RemoveAsync($"product:{productId}");
            
            // Remove any related caches
            await InvalidateAllProductsCacheAsync();
        }
        
        /// <inheritdoc />
        public async Task InvalidateAllProductsCacheAsync()
        {
            // Remove all product list caches
            await _cacheService.RemoveByPrefixAsync("products");
        }
        
        /// <inheritdoc />
        public async Task InvalidateProductsByCategoryAsync(int categoryId)
        {
            // Remove category-specific product caches
            await _cacheService.RemoveByPrefixAsync($"products:category:{categoryId}");
            
            // Also remove the all products cache as it would now be stale
            await InvalidateAllProductsCacheAsync();
        }
    }
}