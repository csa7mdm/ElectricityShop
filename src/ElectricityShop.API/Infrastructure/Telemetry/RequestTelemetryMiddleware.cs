using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.API.Infrastructure.Telemetry
{
    /// <summary>
    /// Middleware to track request metrics
    /// </summary>
    public class RequestTelemetryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTelemetryMiddleware> _logger;

        public RequestTelemetryMiddleware(RequestDelegate next, ILogger<RequestTelemetryMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Increment request counter
            ApiMetrics.TotalRequests.Add(1);
            
            // Track timing
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                // Get user ID if available for active user tracking
                var userId = context.User?.Identity?.Name;
                if (!string.IsNullOrEmpty(userId))
                {
                    TelemetryService.RecordUserActivity(userId);
                }
                
                // Continue with request pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during request processing");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                
                // Record the request duration
                ApiMetrics.RequestDuration.Record(stopwatch.Elapsed.TotalMilliseconds);
                
                // Log slow requests (over 500ms)
                if (stopwatch.ElapsedMilliseconds > 500)
                {
                    _logger.LogWarning("Slow request: {Path} took {ElapsedMs}ms", 
                        context.Request.Path, 
                        stopwatch.ElapsedMilliseconds);
                }
            }
        }
    }
}