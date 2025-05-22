using System;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for cache operations in the application.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Gets a cached item asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of cached item.</typeparam>
        /// <param name="key">Cache key.</param>
        /// <returns>Cached item or null if not found.</returns>
        Task<T> GetAsync<T>(string key) where T : class;

        /// <summary>
        /// Sets an item in cache asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of item to cache.</typeparam>
        /// <param name="key">Cache key.</param>
        /// <param name="value">Value to cache.</param>
        /// <param name="expiry">Optional expiration timespan.</param>
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;

        /// <summary>
        /// Removes an item from cache asynchronously.
        /// </summary>
        /// <param name="key">Cache key to remove.</param>
        Task RemoveAsync(string key);

        /// <summary>
        /// Removes all cache items with keys starting with the specified prefix.
        /// </summary>
        /// <param name="prefixKey">Prefix to match for removal.</param>
        Task RemoveByPrefixAsync(string prefixKey);

        /// <summary>
        /// Checks if an item exists in cache.
        /// </summary>
        /// <param name="key">Cache key to check.</param>
        /// <returns>True if item exists, false otherwise.</returns>
        Task<bool> ExistsAsync(string key);
    }
}