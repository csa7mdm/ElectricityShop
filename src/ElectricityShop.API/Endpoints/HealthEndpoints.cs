using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElectricityShop.API.Endpoints
{
    /// <summary>
    /// Health check endpoints implemented as minimal APIs
    /// </summary>
    public static class HealthEndpoints
    {
        /// <summary>
        /// Maps health check endpoints using minimal API syntax
        /// </summary>
        public static WebApplication MapHealthEndpoints(this WebApplication app)
        {
            // Simple health check endpoint
            app.MapGet("/api/health", async (HttpContext context, HealthCheckService healthCheckService) =>
            {
                var result = await healthCheckService.CheckHealthAsync();
                context.Response.StatusCode = result.Status == HealthStatus.Healthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;
                
                return result.Status.ToString();
            }).WithName("GetHealth")
              .WithTags("Health")
              .WithOpenApi(operation => 
              { 
                  operation.Summary = "Get API health status";
                  operation.Description = "Returns the current health status of the API";
                  return operation;
              });

            // Detailed health check endpoint
            app.MapGet("/api/health/detail", async (HttpContext context, HealthCheckService healthCheckService) =>
            {
                var report = await healthCheckService.CheckHealthAsync();
                
                context.Response.StatusCode = report.Status == HealthStatus.Healthy ? 
                    StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;
                
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
                
                await context.Response.WriteAsJsonAsync(response, options);
            }).WithName("GetHealthDetail")
              .WithTags("Health")
              .WithOpenApi(operation => 
              { 
                  operation.Summary = "Get detailed API health status";
                  operation.Description = "Returns detailed health status information including all health checks";
                  return operation;
              });

            return app;
        }
    }
}