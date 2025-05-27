using System;
using System.Threading.Tasks;
using ElectricityShop.Plugin.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.Plugin.Core
{
    /// <summary>
    /// Extension methods for registering plugin services with the dependency injection container.
    /// </summary>
    public static class PluginServiceCollectionExtensions
    {
        /// <summary>
        /// Adds plugin services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddPluginSystem(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register plugin settings
            services.Configure<PluginSettings>(configuration.GetSection("Plugins"));
            
            // Register plugin services
            services.AddSingleton<IPluginLoader, PluginLoader>();
            
            // Register generic extension executor
            services.AddScoped(typeof(IPluginExtensionExecutor<>), typeof(PluginExtensionExecutor<>));
            
            return services;
        }

        /// <summary>
        /// Loads and registers all plugins from the configured directories.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task LoadPluginsAsync(this IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<IPlugin>>();
            var pluginLoader = serviceProvider.GetRequiredService<IPluginLoader>();
            
            var plugins = await pluginLoader.LoadPluginsAsync();
            
            // Configure services for each plugin
            foreach (var plugin in plugins)
            {
                try
                {
                    logger.LogInformation("Configuring services for plugin {PluginName}", plugin.Name);
                    plugin.ConfigureServices(serviceProvider.GetServices<IServiceCollection>() as IServiceCollection);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error configuring services for plugin {PluginName}: {ErrorMessage}",
                        plugin.Name, ex.Message);
                }
            }
            
            // Initialize each plugin
            foreach (var plugin in plugins)
            {
                try
                {
                    logger.LogInformation("Initializing plugin {PluginName}", plugin.Name);
                    await plugin.InitializeAsync(serviceProvider);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error initializing plugin {PluginName}: {ErrorMessage}",
                        plugin.Name, ex.Message);
                }
            }
        }

        /// <summary>
        /// Shuts down all loaded plugins.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task ShutdownPluginsAsync(this IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<IPlugin>>();
            var pluginLoader = serviceProvider.GetRequiredService<IPluginLoader>();
            
            foreach (var plugin in pluginLoader.LoadedPlugins)
            {
                try
                {
                    logger.LogInformation("Shutting down plugin {PluginName}", plugin.Name);
                    await plugin.ShutdownAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error shutting down plugin {PluginName}: {ErrorMessage}",
                        plugin.Name, ex.Message);
                }
            }
        }
    }
}