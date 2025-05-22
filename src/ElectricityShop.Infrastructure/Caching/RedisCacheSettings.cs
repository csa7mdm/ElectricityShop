namespace ElectricityShop.Infrastructure.Caching
{
    /// <summary>
    /// Configuration settings for Redis cache.
    /// </summary>
    public class RedisCacheSettings
    {
        /// <summary>
        /// Redis connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Instance name prefix for Redis keys.
        /// </summary>
        public string InstanceName { get; set; }

        /// <summary>
        /// Default expiration time in minutes.
        /// </summary>
        public int DefaultExpiryMinutes { get; set; } = 30;
    }
}