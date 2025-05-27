using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using ElectricityShop.Plugin.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElectricityShop.Plugin.Core
{
    /// <summary>
    /// Responsible for loading plugins from the file system.
    /// </summary>
    public class PluginLoader : IPluginLoader
    {
        private readonly ILogger<PluginLoader> _logger;
        private readonly PluginSettings _settings;
        private readonly List<IPlugin> _loadedPlugins = new List<IPlugin>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginLoader"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The plugin settings options.</param>
        public PluginLoader(ILogger<PluginLoader> logger, IOptions<PluginSettings> options)
        {
            _logger = logger;
            _settings = options.Value;
        }

        /// <summary>
        /// Gets all loaded plugins.
        /// </summary>
        public IReadOnlyCollection<IPlugin> LoadedPlugins => _loadedPlugins.AsReadOnly();

        /// <summary>
        /// Loads all plugins from the configured directories.
        /// </summary>
        /// <returns>A collection of loaded plugins.</returns>
        public async Task<IReadOnlyCollection<IPlugin>> LoadPluginsAsync()
        {
            _loadedPlugins.Clear();

            var pluginDirectories = new List<string>();
            
            // Add the main plugin directory if it exists
            if (!string.IsNullOrEmpty(_settings.PluginsDirectory) && Directory.Exists(_settings.PluginsDirectory))
            {
                pluginDirectories.Add(_settings.PluginsDirectory);
            }
            
            // Add any additional plugin directories
            if (_settings.AdditionalPluginDirectories != null)
            {
                foreach (var directory in _settings.AdditionalPluginDirectories)
                {
                    if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                    {
                        pluginDirectories.Add(directory);
                    }
                }
            }

            if (!pluginDirectories.Any())
            {
                _logger.LogWarning("No valid plugin directories found. No plugins will be loaded.");
                return _loadedPlugins;
            }

            foreach (var pluginDirectory in pluginDirectories)
            {
                _logger.LogInformation("Loading plugins from directory: {Directory}", pluginDirectory);
                await LoadPluginsFromDirectoryAsync(pluginDirectory);
            }

            _logger.LogInformation("Loaded {Count} plugins", _loadedPlugins.Count);
            return _loadedPlugins;
        }

        private async Task LoadPluginsFromDirectoryAsync(string directory)
        {
            var pluginManager = new PluginManager(_settings);
            var pluginAssemblies = pluginManager.DiscoverPlugins(directory);

            foreach (var pluginInfo in pluginAssemblies)
            {
                try
                {
                    if (!pluginInfo.IsEnabled)
                    {
                        _logger.LogInformation("Plugin {PluginName} is disabled and will not be loaded", pluginInfo.Name);
                        continue;
                    }

                    var assembly = LoadPluginAssembly(Path.Combine(directory, pluginInfo.AssemblyFileName));
                    var pluginType = assembly.GetType(pluginInfo.TypeName);

                    if (pluginType == null)
                    {
                        _logger.LogError("Could not find plugin type {TypeName} in assembly {AssemblyName}", 
                            pluginInfo.TypeName, pluginInfo.AssemblyFileName);
                        continue;
                    }

                    if (!typeof(IPlugin).IsAssignableFrom(pluginType))
                    {
                        _logger.LogError("Type {TypeName} does not implement IPlugin", pluginInfo.TypeName);
                        continue;
                    }

                    var plugin = (IPlugin)Activator.CreateInstance(pluginType);
                    
                    // Check if the plugin is already loaded
                    if (_loadedPlugins.Any(p => p.Id == plugin.Id))
                    {
                        _logger.LogWarning("Plugin {PluginId} is already loaded. Skipping duplicate plugin.", plugin.Id);
                        continue;
                    }
                    
                    _loadedPlugins.Add(plugin);
                    _logger.LogInformation("Loaded plugin {PluginName} ({PluginVersion}) from {PluginFile}",
                        plugin.Name, plugin.Version, pluginInfo.AssemblyFileName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading plugin {PluginName}: {ErrorMessage}", 
                        pluginInfo.Name, ex.Message);
                }
            }
        }

        private Assembly LoadPluginAssembly(string pluginPath)
        {
            _logger.LogDebug("Loading assembly from {PluginPath}", pluginPath);
            var loadContext = new PluginLoadContext(pluginPath);
            return loadContext.LoadFromAssemblyPath(pluginPath);
        }

        /// <summary>
        /// Custom assembly load context for loading plugin assemblies.
        /// </summary>
        private class PluginLoadContext : AssemblyLoadContext
        {
            private readonly AssemblyDependencyResolver _resolver;

            public PluginLoadContext(string pluginPath)
            {
                _resolver = new AssemblyDependencyResolver(pluginPath);
            }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
                return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
            }

            protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
            {
                var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
                return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
            }
        }
    }
}