using ElectricityShop.API.HealthChecks;
using ElectricityShop.API.Infrastructure.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElectricityShop.API.Extensions
{
    /// <summary>
    /// Extensions for configuring API health checks
    /// </summary>
    public static class HealthCheckExtensions
    {
        /// <summary>
        /// Adds custom health checks to the health checks builder
        /// </summary>
        public static IHealthChecksBuilder AddCustomHealthChecks(this IHealthChecksBuilder healthChecksBuilder)
        {
            // Add database health check
            healthChecksBuilder.AddCheck<DatabaseHealthCheck>("database_check", 
                failureStatus: HealthStatus.Degraded, 
                tags: new[] { "database", "sql", "ready" });
            
            // Add RabbitMQ health check
            healthChecksBuilder.AddCheck<RabbitMQHealthCheck>("rabbitmq_check", 
                failureStatus: HealthStatus.Degraded, 
                tags: new[] { "rabbitmq", "messaging", "ready" });
            
            // Add a simple ping health check
            healthChecksBuilder.AddCheck("ping_check", () => 
                HealthCheckResult.Healthy("API is responding"), 
                tags: new[] { "ping", "ready" });
            
            // Add OpenTelemetry health check
            healthChecksBuilder.AddCheck<OpenTelemetryHealthCheck>("opentelemetry_check",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "telemetry", "monitoring", "ready" });
            
            return healthChecksBuilder;
        }

        /// <summary>
        /// Adds health check middleware to the application pipeline
        /// </summary>
        public static IApplicationBuilder UseCustomHealthChecks(this IApplicationBuilder app)
        {
            // Simple health check endpoint
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                // Simple text result
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync(report.Status.ToString());
                }
            });

            // Detailed health check endpoint
            app.UseHealthChecks("/health/detail", new HealthCheckOptions
            {
                // Return detailed JSON
                ResponseWriter = WriteDetailedHealthCheckResponse,
                // Optional: filter health checks by tag
                Predicate = _ => true,
                // Results for degraded status and worse
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
            });

            // Readiness probe (used by Kubernetes)
            app.UseHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = (check) => check.Tags.Contains("ready"),
                ResponseWriter = WriteDetailedHealthCheckResponse
            });

            // Liveness probe (used by Kubernetes)
            app.UseHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = (_) => true,
                ResponseWriter = WriteDetailedHealthCheckResponse
            });

            return app;
        }

        /// <summary>
        /// Writes detailed health check response in JSON format
        /// </summary>
        private static Task WriteDetailedHealthCheckResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = report.Status.ToString(),
                totalDuration = report.TotalDuration.TotalMilliseconds,
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    duration = e.Value.Duration.TotalMilliseconds,
                    description = e.Value.Description,
                    data = e.Value.Data,
                    tags = e.Value.Tags
                })
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}