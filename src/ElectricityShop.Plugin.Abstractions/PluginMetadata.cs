using System;
using System.Collections.Generic;

namespace ElectricityShop.Plugin.Abstractions
{
    /// <summary>
    /// Contains metadata about a plugin.
    /// </summary>
    public class PluginMetadata
    {
        /// <summary>
        /// Gets or sets the unique identifier for this plugin.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of this plugin.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version of this plugin.
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the author of this plugin.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets a description of this plugin.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the assembly file name of this plugin.
        /// </summary>
        public string AssemblyFileName { get; set; }

        /// <summary>
        /// Gets or sets the full type name of the plugin implementation.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this plugin is enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets additional configuration for this plugin.
        /// </summary>
        public PluginConfigurationData ConfigurationData { get; set; } = new PluginConfigurationData();
    }

    /// <summary>
    /// Contains configuration data for a plugin.
    /// </summary>
    public class PluginConfigurationData
    {
        /// <summary>
        /// Gets or sets additional settings for this plugin.
        /// </summary>
        public IDictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
    }
}