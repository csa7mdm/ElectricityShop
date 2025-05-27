using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ElectricityShop.Plugin.Abstractions;

namespace ElectricityShop.Plugin.Core
{
    /// <summary>
    /// Manages plugin discovery and configuration.
    /// </summary>
    public class PluginManager
    {
        private readonly PluginSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManager"/> class.
        /// </summary>
        /// <param name="settings">The plugin settings.</param>
        public PluginManager(PluginSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Discovers available plugins in the specified directory.
        /// </summary>
        /// <param name="directory">The directory to search for plugins.</param>
        /// <returns>A collection of plugin metadata.</returns>
        public IEnumerable<PluginMetadata> DiscoverPlugins(string directory)
        {
            var pluginManifests = Directory.GetFiles(directory, "*.plugin.json", SearchOption.AllDirectories);
            
            var plugins = new List<PluginMetadata>();
            
            foreach (var manifestPath in pluginManifests)
            {
                try
                {
                    var json = File.ReadAllText(manifestPath);
                    var plugin = JsonSerializer.Deserialize<PluginMetadata>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (plugin != null)
                    {
                        // Ensure the assembly file exists
                        var assemblyPath = Path.Combine(Path.GetDirectoryName(manifestPath), plugin.AssemblyFileName);
                        if (!File.Exists(assemblyPath))
                        {
                            continue;
                        }
                        
                        // Check if this plugin ID is in the disabled list
                        if (_settings.DisabledPlugins?.Contains(plugin.Id) == true)
                        {
                            plugin.IsEnabled = false;
                        }
                        
                        // Apply any custom configuration from settings
                        if (_settings.PluginConfigurations?.TryGetValue(plugin.Id, out var config) == true)
                        {
                            plugin.ConfigurationData = config;
                        }
                        
                        plugins.Add(plugin);
                    }
                }
                catch (Exception)
                {
                    // Skip invalid plugin manifests
                    continue;
                }
            }
            
            return plugins;
        }
    }
}