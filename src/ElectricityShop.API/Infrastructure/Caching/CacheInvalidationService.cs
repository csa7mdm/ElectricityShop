using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

namespace ElectricityShop.API.Infrastructure.Caching
{
    /// <summary>
    /// Service for invalidating cache entries when entities change
    /// </summary>
    public class CacheInvalidationService
    {
        private readonly ICacheService _cacheService;

        public CacheInvalidationService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        /// <summary>
        /// Invalidates product-related cache entries
        /// </summary>
        public async Task InvalidateProductCacheAsync(Guid productId)
        {
            // Invalidate specific product
            await _cacheService.RemoveAsync(CacheKeys.ProductDetails(productId));

            // Invalidate product lists (we can't know which lists contain this product, so invalidate all)
            // In a real-world scenario, we might want to be more selective based on categories, etc.
            await _cacheService.RemoveAsync("products_list_*");
        }

        /// <summary>
        /// Invalidates category-related cache entries
        /// </summary>
        public async Task InvalidateCategoryCacheAsync(Guid categoryId)
        {
            // Invalidate category lists
            await _cacheService.RemoveAsync(CacheKeys.CategoriesList);

            // Invalidate product lists that might contain products from this category
            await _cacheService.RemoveAsync($"products_list_*_c{categoryId}");
        }

        /// <summary>
        /// Invalidates order-related cache entries
        /// </summary>
        public async Task InvalidateOrderCacheAsync(Guid userId, Guid orderId)
        {
            // Invalidate user's orders list
            await _cacheService.RemoveAsync($"orders_user_{userId}");

            // Invalidate specific order
            await _cacheService.RemoveAsync($"order_{orderId}");
        }

        /// <summary>
        /// Invalidates cart-related cache entries
        /// </summary>
        public async Task InvalidateCartCacheAsync(Guid userId)
        {
            await _cacheService.RemoveAsync($"cart_user_{userId}");
        }
    }
}