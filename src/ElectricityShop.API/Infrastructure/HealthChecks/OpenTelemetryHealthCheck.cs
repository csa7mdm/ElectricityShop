using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.API.Infrastructure.HealthChecks
{
    /// <summary>
    /// Health check for OpenTelemetry
    /// </summary>
    public class OpenTelemetryHealthCheck : IHealthCheck
    {
        private readonly ILogger<OpenTelemetryHealthCheck> _logger;

        public OpenTelemetryHealthCheck(ILogger<OpenTelemetryHealthCheck> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if OTLP endpoint is configured
                var otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
                
                if (string.IsNullOrEmpty(otlpEndpoint))
                {
                    return Task.FromResult(
                        HealthCheckResult.Degraded(
                            "OTLP endpoint not configured. Using console exporter instead."
                        )
                    );
                }

                return Task.FromResult(HealthCheckResult.Healthy("OpenTelemetry is configured correctly"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking OpenTelemetry health");
                return Task.FromResult(
                    HealthCheckResult.Unhealthy("Error checking OpenTelemetry health", ex)
                );
            }
        }
    }
}