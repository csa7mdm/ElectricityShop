using ElectricityShop.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricityShop.Infrastructure.Caching
{
    /// <summary>
    /// Redis implementation of the cache service.
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly RedisCacheSettings _settings;
        private readonly CacheStatistics _statistics;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheService"/> class.
        /// </summary>
        /// <param name="redis">Redis connection multiplexer.</param>
        /// <param name="settings">Redis cache settings.</param>
        /// <param name="statistics">Cache statistics tracker.</param>
        public RedisCacheService(
            IConnectionMultiplexer redis, 
            IOptions<RedisCacheSettings> settings,
            CacheStatistics statistics)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
            _database = _redis.GetDatabase();
        }

        /// <inheritdoc />
        public async Task<T> GetAsync<T>(string key) where T : class
        {
            var value = await _database.StringGetAsync(GetFullKey(key));
            if (value.IsNullOrEmpty)
            {
                _statistics.RecordMiss(key);
                return null;
            }

            _statistics.RecordHit(key);
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <inheritdoc />
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
        {
            var expiryTime = expiry ?? TimeSpan.FromMinutes(_settings.DefaultExpiryMinutes);
            var serializedValue = JsonConvert.SerializeObject(value);
            await _database.StringSetAsync(GetFullKey(key), serializedValue, expiryTime);
        }

        /// <inheritdoc />
        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(GetFullKey(key));
        }

        /// <inheritdoc />
        public async Task RemoveByPrefixAsync(string prefixKey)
        {
            var prefix = GetFullKey(prefixKey);
            var endpoints = _redis.GetEndPoints();
            
            // Get all keys matching the prefix pattern
            var keys = new List<RedisKey>();
            foreach (var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint);
                keys.AddRange(server.Keys(pattern: $"{prefix}*"));
            }
            
            if (keys.Count > 0)
            {
                await _database.KeyDeleteAsync(keys.ToArray());
            }
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(string key)
        {
            return await _database.KeyExistsAsync(GetFullKey(key));
        }

        private string GetFullKey(string key)
        {
            return $"{_settings.InstanceName}{key}";
        }
    }
}