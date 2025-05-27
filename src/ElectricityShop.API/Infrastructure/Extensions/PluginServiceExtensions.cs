using System;
using System.Threading.Tasks;
using ElectricityShop.Plugin.Abstractions;
using ElectricityShop.Plugin.Abstractions.ExtensionPoints;
using ElectricityShop.Plugin.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.API.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for registering and using plugins in the application.
    /// </summary>
    public static class PluginServiceExtensions
    {
        /// <summary>
        /// Adds the plugin system to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddPluginSystem(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add plugin core services
            services.AddPluginSystem(configuration);

            // Register extension point resolvers
            services.AddScoped(typeof(IPluginExtensionExecutor<>), typeof(PluginExtensionExecutor<>));
            
            // Register specific extension point interfaces
            services.AddScoped<IPluginExtensionExecutor<IProductExtensionPoint>, PluginExtensionExecutor<IProductExtensionPoint>>();
            services.AddScoped<IPluginExtensionExecutor<IRecommendationExtensionPoint>, PluginExtensionExecutor<IRecommendationExtensionPoint>>();

            return services;
        }

        /// <summary>
        /// Configures the application to use the plugin system.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UsePluginSystem(this IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger<IPlugin>>();
            
            // Add middleware for plugin initialization and shutdown
            app.Use(async (context, next) =>
            {
                // This middleware doesn't do anything with the request pipeline
                // It just ensures we have initialized the plugins
                await next();
            });

            // Initialize plugins on application startup
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            
            lifetime.ApplicationStarted.Register(async () =>
            {
                try
                {
                    logger.LogInformation("Initializing plugin system...");
                    await app.ApplicationServices.LoadPluginsAsync();
                    logger.LogInformation("Plugin system initialized");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error initializing plugin system");
                }
            });

            lifetime.ApplicationStopping.Register(async () =>
            {
                try
                {
                    logger.LogInformation("Shutting down plugin system...");
                    await app.ApplicationServices.ShutdownPluginsAsync();
                    logger.LogInformation("Plugin system shut down");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error shutting down plugin system");
                }
            });

            return app;
        }
    }
}