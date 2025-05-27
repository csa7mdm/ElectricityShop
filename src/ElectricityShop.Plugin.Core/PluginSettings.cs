using System.Collections.Generic;
using ElectricityShop.Plugin.Abstractions;

namespace ElectricityShop.Plugin.Core
{
    /// <summary>
    /// Contains settings for the plugin system.
    /// </summary>
    public class PluginSettings
    {
        /// <summary>
        /// Gets or sets the directory containing plugins.
        /// </summary>
        public string PluginsDirectory { get; set; } = "Plugins";

        /// <summary>
        /// Gets or sets a value indicating whether plugins should be loaded at application startup.
        /// </summary>
        public bool LoadPluginsAtStartup { get; set; } = true;

        /// <summary>
        /// Gets or sets additional directories to search for plugins.
        /// </summary>
        public List<string> AdditionalPluginDirectories { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a list of plugin IDs that should be disabled.
        /// </summary>
        public List<string> DisabledPlugins { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets custom configuration for specific plugins.
        /// </summary>
        public Dictionary<string, PluginConfigurationData> PluginConfigurations { get; set; } = new Dictionary<string, PluginConfigurationData>();
    }
}