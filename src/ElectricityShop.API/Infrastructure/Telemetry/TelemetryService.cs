using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.API.Infrastructure.Telemetry
{
    /// <summary>
    /// Service that provides telemetry data for observability
    /// </summary>
    public class TelemetryService
    {
        private static readonly ConcurrentDictionary<string, DateTime> _activeUsers = new();
        private static readonly ConcurrentQueue<(decimal amount, DateTime timestamp)> _orders = new();
        private static int _productCount = 0;
        private static readonly Process _currentProcess = Process.GetCurrentProcess();
        private static readonly ILogger<TelemetryService> _logger = CreateLogger();

        // Static method to initialize logger
        private static ILogger<TelemetryService> CreateLogger()
        {
        // This would typically be injected, but for static context we're creating a minimal logger
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        return loggerFactory.CreateLogger<TelemetryService>();
        }

        /// <summary>
        /// Records a user activity to track active users
        /// </summary>
        /// <param name="userId">The user identifier</param>
        public static void RecordUserActivity(string userId)
        {
            _activeUsers.AddOrUpdate(userId, DateTime.UtcNow, (_, _) => DateTime.UtcNow);
        }

        /// <summary>
        /// Records a completed order
        /// </summary>
        /// <param name="orderAmount">The total order amount</param>
        public static void RecordOrder(decimal orderAmount)
        {
            _orders.Enqueue((orderAmount, DateTime.UtcNow));
            
            // Keep only last 24 hours of orders
            CleanupOldOrders();
        }

        /// <summary>
        /// Updates the product count
        /// </summary>
        /// <param name="count">The current product count</param>
        public static void UpdateProductCount(int count)
        {
            Interlocked.Exchange(ref _productCount, count);
        }

        /// <summary>
        /// Gets the count of active users in the last 15 minutes
        /// </summary>
        /// <returns>Number of active users</returns>
        public static int GetActiveUsers()
        {
            var threshold = DateTime.UtcNow.AddMinutes(-15);
            return _activeUsers.Count(u => u.Value >= threshold);
        }

        /// <summary>
        /// Gets the average order value for the last 24 hours
        /// </summary>
        /// <returns>Average order value or 0 if no orders</returns>
        public static double GetAverageOrderValue()
        {
            var threshold = DateTime.UtcNow.AddHours(-24);
            var recentOrders = _orders.Where(o => o.timestamp >= threshold).ToList();
            
            if (recentOrders.Count == 0)
                return 0;
                
            return (double)recentOrders.Average(o => o.amount);
        }

        /// <summary>
        /// Gets the current product count
        /// </summary>
        /// <returns>Number of active products</returns>
        public static int GetProductCount()
        {
            return _productCount;
        }

        /// <summary>
        /// Gets the current CPU usage percentage
        /// </summary>
        /// <returns>CPU usage as a percentage</returns>
        public static double GetCpuUsage()
        {
            try
            {
                _currentProcess.Refresh();
                return Math.Round(_currentProcess.TotalProcessorTime.TotalMilliseconds / 
                       (Environment.ProcessorCount * _currentProcess.UserProcessorTime.TotalMilliseconds) * 100, 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting CPU usage");
                return 0;
            }
        }

        /// <summary>
        /// Gets the current memory usage in MB
        /// </summary>
        /// <returns>Memory usage in MB</returns>
        public static double GetMemoryUsage()
        {
            try
            {
                _currentProcess.Refresh();
                return Math.Round(_currentProcess.WorkingSet64 / 1024.0 / 1024.0, 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting memory usage");
                return 0;
            }
        }

        /// <summary>
        /// Cleans up orders older than 24 hours
        /// </summary>
        private static void CleanupOldOrders()
        {
            var threshold = DateTime.UtcNow.AddHours(-24);
            
            while (_orders.TryPeek(out var oldestOrder) && 
                  oldestOrder.timestamp < threshold && 
                  _orders.Count > 0)
            {
                _orders.TryDequeue(out _);
            }
        }
    }
}