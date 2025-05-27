using System.Collections.Generic;
using System.Threading.Tasks;
using ElectricityShop.Plugin.Abstractions;

namespace ElectricityShop.Plugin.Core
{
    /// <summary>
    /// Interface for the plugin loader.
    /// </summary>
    public interface IPluginLoader
    {
        /// <summary>
        /// Gets all loaded plugins.
        /// </summary>
        IReadOnlyCollection<IPlugin> LoadedPlugins { get; }

        /// <summary>
        /// Loads all plugins from the configured directories.
        /// </summary>
        /// <returns>A collection of loaded plugins.</returns>
        Task<IReadOnlyCollection<IPlugin>> LoadPluginsAsync();
    }
}