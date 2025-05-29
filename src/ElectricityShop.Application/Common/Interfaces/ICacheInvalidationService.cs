using System;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Common.Interfaces
{
    /// <summary>
    /// Service responsible for invalidating cache items based on business rules.
    /// </summary>
    public interface ICacheInvalidationService
    {
        /// <summary>
        /// Invalidates cache for a specific product.
        /// </summary>
        /// <param name="productId">ID of the product to invalidate cache for.</param>
        Task InvalidateProductCacheAsync(Guid productId);

        /// <summary>
        /// Invalidates cache for all products.
        /// </summary>
        Task InvalidateAllProductsCacheAsync();

        /// <summary>
        /// Invalidates cache for products in a specific category.
        /// </summary>
        /// <param name="categoryId">ID of the category to invalidate product cache for.</param>
        Task InvalidateProductsByCategoryAsync(Guid categoryId);
    }
}