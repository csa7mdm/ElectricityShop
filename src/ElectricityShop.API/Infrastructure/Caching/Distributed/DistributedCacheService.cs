using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElectricityShop.API.Infrastructure.Caching.Distributed
{
    /// <summary>
    /// Service for working with distributed cache
    /// </summary>
    public class DistributedCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public DistributedCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        /// <summary>
        /// Gets an item from the cache
        /// </summary>
        public async Task<T> GetAsync<T>(string key)
        {
            var cachedValue = await _distributedCache.GetStringAsync(key);

            if (string.IsNullOrEmpty(cachedValue))
                return default;

            return JsonSerializer.Deserialize<T>(cachedValue);
        }

        /// <summary>
        /// Sets an item in the cache
        /// </summary>
        public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null)
        {
            var options = new DistributedCacheEntryOptions();

            if (absoluteExpiration.HasValue)
                options.SetAbsoluteExpiration(absoluteExpiration.Value);
            else
                options.SetAbsoluteExpiration(TimeSpan.FromMinutes(5)); // Default expiration

            var serializedValue = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, serializedValue, options);
        }

        /// <summary>
        /// Removes an item from the cache
        /// </summary>
        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        /// <summary>
        /// Gets or sets an item in the cache
        /// </summary>
        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null)
        {
            var cachedValue = await GetAsync<T>(key);

            if (cachedValue != null)
                return cachedValue;

            var value = await factory();

            if (value != null)
                await SetAsync(key, value, absoluteExpiration);

            return value;
        }
    }
}