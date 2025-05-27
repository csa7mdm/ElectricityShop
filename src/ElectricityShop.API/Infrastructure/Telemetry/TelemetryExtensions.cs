using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Diagnostics;

namespace ElectricityShop.API.Infrastructure.Telemetry
{
    /// <summary>
    /// Extension methods for telemetry configuration
    /// </summary>
    public static class TelemetryExtensions
    {
        /// <summary>
        /// Adds OpenTelemetry services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="serviceName">The service name</param>
        /// <param name="serviceVersion">The service version</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddOpenTelemetry(
            this IServiceCollection services,
            string serviceName,
            string serviceVersion)
        {
            // Create a resource with service information
            var resource = ResourceBuilder.CreateDefault()
                .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
                .AddAttributes(new System.Collections.Generic.KeyValuePair<string, object>[] {
                    new System.Collections.Generic.KeyValuePair<string, object>("deployment.environment",
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production")
                });

            // Add metrics
            services.AddOpenTelemetry()
                .WithMetrics(builder =>
                {
                    builder
                        .SetResourceBuilder(resource)
                        .AddMeter("ElectricityShop.API")
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddProcessInstrumentation();

                    // Configure exporters based on environment variables
                    if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")))
                    {
                        builder.AddOtlpExporter();
                    }
                    else
                    {
                        builder.AddConsoleExporter();
                    }
                });

            // Add tracing
            services.AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    builder
                        .SetResourceBuilder(resource)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddSource("ElectricityShop.API");

                    // Configure exporters based on environment variables
                    if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")))
                    {
                        builder.AddOtlpExporter();
                    }
                    else
                    {
                        builder.AddConsoleExporter();
                    }
                });

            return services;
        }

        /// <summary>
        /// Adds the request telemetry middleware to the application pipeline
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <returns>The application builder for chaining</returns>
        public static IApplicationBuilder UseRequestTelemetry(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestTelemetryMiddleware>();
        }
    }
}