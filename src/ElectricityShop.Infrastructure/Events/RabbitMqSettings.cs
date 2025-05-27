namespace ElectricityShop.Infrastructure.Events
{
    /// <summary>
    /// RabbitMQ connection settings
    /// </summary>
    public class RabbitMqSettings
    {
        /// <summary>
        /// Gets or sets the RabbitMQ host name
        /// </summary>
        public string HostName { get; set; } = "localhost";
        
        /// <summary>
        /// Gets or sets the RabbitMQ port
        /// </summary>
        public int Port { get; set; } = 5672;
        
        /// <summary>
        /// Gets or sets the RabbitMQ user name
        /// </summary>
        public string UserName { get; set; } = "guest";
        
        /// <summary>
        /// Gets or sets the RabbitMQ password
        /// </summary>
        public string Password { get; set; } = "guest";
        
        /// <summary>
        /// Gets or sets the RabbitMQ virtual host
        /// </summary>
        public string VirtualHost { get; set; } = "/";
        
        /// <summary>
        /// Gets or sets a value indicating whether to use SSL connection
        /// </summary>
        public bool UseSsl { get; set; } = false;
        
        /// <summary>
        /// Gets or sets the exchange name
        /// </summary>
        public string ExchangeName { get; set; } = "electricity_shop_exchange";
        
        /// <summary>
        /// Gets or sets the dead letter exchange name
        /// </summary>
        public string DeadLetterExchange { get; set; } = "electricity_shop_dead_letter_exchange";
        
        /// <summary>
        /// Gets or sets the number of retries for failed messages
        /// </summary>
        public int RetryCount { get; set; } = 3;
        
        /// <summary>
        /// Gets or sets the retry interval in seconds
        /// </summary>
        public int RetryIntervalSeconds { get; set; } = 30;
        
        /// <summary>
        /// Gets or sets the connection timeout in seconds
        /// </summary>
        public int ConnectionTimeoutSeconds { get; set; } = 15;
        
        /// <summary>
        /// Gets or sets the prefetch count
        /// </summary>
        public ushort PrefetchCount { get; set; } = 10;
    }
}