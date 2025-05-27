using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ElectricityShop.API.Infrastructure.Telemetry
{
    /// <summary>
    /// API metrics for monitoring
    /// </summary>
    public static class ApiMetrics
    {
        // Source for all metrics
        private static readonly Meter s_meter = new Meter("ElectricityShop.API", "1.0.0");

        // Request metrics
        public static readonly Counter<long> TotalRequests = s_meter.CreateCounter<long>("api.requests.total", description: "Total number of requests received");
        public static readonly Histogram<double> RequestDuration = s_meter.CreateHistogram<double>("api.requests.duration", unit: "ms", description: "Duration of requests");

        // Authentication metrics
        public static readonly Counter<long> SuccessfulLogins = s_meter.CreateCounter<long>("api.auth.logins.successful", description: "Number of successful login attempts");
        public static readonly Counter<long> FailedLogins = s_meter.CreateCounter<long>("api.auth.logins.failed", description: "Number of failed login attempts");
        public static readonly Counter<long> Registrations = s_meter.CreateCounter<long>("api.auth.registrations", description: "Number of user registrations");

        // Product metrics
        public static readonly Counter<long> ProductViews = s_meter.CreateCounter<long>("api.products.views", description: "Number of product views");
        public static readonly Counter<long> ProductSearches = s_meter.CreateCounter<long>("api.products.searches", description: "Number of product searches");
        public static readonly Counter<long> ProductCreations = s_meter.CreateCounter<long>("api.products.creations", description: "Number of products created");
        public static readonly Counter<long> ProductUpdates = s_meter.CreateCounter<long>("api.products.updates", description: "Number of products updated");

        // Order metrics
        public static readonly Counter<long> OrdersCreated = s_meter.CreateCounter<long>("api.orders.created", description: "Number of orders created");
        public static readonly Counter<long> OrdersCompleted = s_meter.CreateCounter<long>("api.orders.completed", description: "Number of orders completed");
        public static readonly Counter<long> OrdersCancelled = s_meter.CreateCounter<long>("api.orders.cancelled", description: "Number of orders cancelled");

        // Cache metrics
        public static readonly Counter<long> CacheHits = s_meter.CreateCounter<long>("api.cache.hits", description: "Number of cache hits");
        public static readonly Counter<long> CacheMisses = s_meter.CreateCounter<long>("api.cache.misses", description: "Number of cache misses");

        // Business metrics
        public static readonly ObservableGauge<int> ActiveUsers = s_meter.CreateObservableGauge<int>(
            "api.business.active_users", 
            () => TelemetryService.GetActiveUsers(), 
            description: "Number of active users in the last 15 minutes"
        );
        
        public static readonly ObservableGauge<double> AverageOrderValue = s_meter.CreateObservableGauge<double>(
            "api.business.average_order_value",
            () => TelemetryService.GetAverageOrderValue(),
            description: "Average order value in the last 24 hours"
        );
        
        public static readonly ObservableGauge<int> ProductCount = s_meter.CreateObservableGauge<int>(
            "api.business.product_count",
            () => TelemetryService.GetProductCount(),
            description: "Total number of active products"
        );

        // System metrics
        public static readonly ObservableGauge<double> CpuUsage = s_meter.CreateObservableGauge<double>(
            "api.system.cpu_usage",
            () => TelemetryService.GetCpuUsage(),
            description: "Current CPU usage percentage"
        );
        
        public static readonly ObservableGauge<double> MemoryUsage = s_meter.CreateObservableGauge<double>(
            "api.system.memory_usage",
            () => TelemetryService.GetMemoryUsage(),
            description: "Current memory usage in MB"
        );
    }
}