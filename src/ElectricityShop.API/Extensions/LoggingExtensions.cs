using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ElectricityShop.API.Extensions
{
    /// <summary>
    /// Extensions for configuring logging
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// Configures advanced logging options for the application
        /// </summary>
        public static ILoggingBuilder ConfigureLogging(this ILoggingBuilder logging)
        {
            // Add custom log providers if needed
            // logging.AddProvider(new CustomLogProvider());
            
            // Configure logging minimum levels by category
            logging.SetMinimumLevel(LogLevel.Information);
            
            logging.AddFilter("Microsoft", LogLevel.Warning);
            logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information);
            logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
            logging.AddFilter("Microsoft.AspNetCore.Mvc", LogLevel.Warning);
            
            // Set application-specific logging levels
            logging.AddFilter("ElectricityShop", LogLevel.Information);
            logging.AddFilter("ElectricityShop.API.Controllers", LogLevel.Debug);
            logging.AddFilter("ElectricityShop.Infrastructure.Identity", LogLevel.Warning);
            
            // Enable sensitive data logging only in Development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                logging.AddFilter("ElectricityShop.Infrastructure.Persistence", LogLevel.Debug);
            }
            else
            {
                logging.AddFilter("ElectricityShop.Infrastructure.Persistence", LogLevel.Warning);
            }

            // Configure other logging options as needed
            return logging;
        }
    }
}