using System;
using System.Threading.Tasks;

namespace ElectricityShop.API.Infrastructure.Caching
{
    /// <summary>
    /// Interface for cache service implementation
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Gets an item from the cache
        /// </summary>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Sets an item in the cache
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null);

        /// <summary>
        /// Removes an item from the cache
        /// </summary>
        Task RemoveAsync(string key);

        /// <summary>
        /// Gets or sets an item in the cache
        /// </summary>
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null);
    }
}