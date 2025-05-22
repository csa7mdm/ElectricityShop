using ElectricityShop.Infrastructure.Messaging.RabbitMQ;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.API.HealthChecks
{
    /// <summary>
    /// Health check for RabbitMQ connection
    /// </summary>
    public class RabbitMQHealthCheck : IHealthCheck
    {
        private readonly RabbitMQConnection _rabbitMQConnection;
        private readonly ILogger<RabbitMQHealthCheck> _logger;

        public RabbitMQHealthCheck(RabbitMQConnection rabbitMQConnection, ILogger<RabbitMQHealthCheck> logger)
        {
            _rabbitMQConnection = rabbitMQConnection;
            _logger = logger;
        }

        /// <summary>
        /// Checks if the RabbitMQ connection is healthy
        /// </summary>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_rabbitMQConnection.IsConnected)
                {
                    return Task.FromResult(HealthCheckResult.Healthy("RabbitMQ connection is healthy"));
                }

                return Task.FromResult(HealthCheckResult.Degraded("RabbitMQ connection is degraded"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ health check failed");
                return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ connection is unhealthy", ex));
            }
        }
    }
}