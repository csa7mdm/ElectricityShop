using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ElectricityShop.Plugin.Abstractions
{
    /// <summary>
    /// Represents the base interface for all plugins in the ElectricityShop system.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Gets the unique identifier for this plugin.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the name of this plugin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the version of this plugin.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Gets the author of this plugin.
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Gets a description of this plugin.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Configures services for this plugin.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <returns>The configured service collection.</returns>
        IServiceCollection ConfigureServices(IServiceCollection services);

        /// <summary>
        /// Initializes this plugin.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task InitializeAsync(IServiceProvider serviceProvider);

        /// <summary>
        /// Shuts down this plugin.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ShutdownAsync();
    }
}