using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ElectricityShop.Infrastructure.Caching
{
    /// <summary>
    /// Tracks cache performance metrics.
    /// </summary>
    public class CacheStatistics
    {
        private readonly ConcurrentDictionary<string, CacheMetrics> _metrics = new();
        
        /// <summary>
        /// Records a cache hit for the specified key.
        /// </summary>
        /// <param name="cacheKey">Cache key that was hit.</param>
        public void RecordHit(string cacheKey)
        {
            GetOrCreateMetrics(cacheKey).Hits++;
        }
        
        /// <summary>
        /// Records a cache miss for the specified key.
        /// </summary>
        /// <param name="cacheKey">Cache key that was missed.</param>
        public void RecordMiss(string cacheKey)
        {
            GetOrCreateMetrics(cacheKey).Misses++;
        }
        
        /// <summary>
        /// Gets all recorded cache metrics.
        /// </summary>
        /// <returns>Dictionary of cache keys and their metrics.</returns>
        public IDictionary<string, CacheMetrics> GetAllMetrics()
        {
            return _metrics;
        }
        
        private CacheMetrics GetOrCreateMetrics(string cacheKey)
        {
            return _metrics.GetOrAdd(cacheKey, _ => new CacheMetrics());
        }
    }
    
    /// <summary>
    /// Metrics for a specific cache key.
    /// </summary>
    public class CacheMetrics
    {
        /// <summary>
        /// Number of cache hits.
        /// </summary>
        public long Hits { get; set; }
        
        /// <summary>
        /// Number of cache misses.
        /// </summary>
        public long Misses { get; set; }
        
        /// <summary>
        /// Ratio of hits to total accesses.
        /// </summary>
        public double HitRatio => Hits + Misses > 0 ? (double)Hits / (Hits + Misses) : 0;
    }
}