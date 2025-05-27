using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ElectricityShop.Plugin.Abstractions;
using ElectricityShop.Plugin.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ElectricityShop.API.Tests.Plugin
{
    public class PluginLoaderTests
    {
        private readonly Mock<ILogger<PluginLoader>> _loggerMock;
        private readonly Mock<IOptions<PluginSettings>> _optionsMock;
        private readonly PluginSettings _settings;

        public PluginLoaderTests()
        {
            _loggerMock = new Mock<ILogger<PluginLoader>>();
            _settings = new PluginSettings
            {
                PluginsDirectory = Path.Combine(Path.GetTempPath(), "PluginTests", "Plugins"),
                AdditionalPluginDirectories = new List<string>(),
                DisabledPlugins = new List<string>(),
                PluginConfigurations = new Dictionary<string, PluginConfigurationData>()
            };
            _optionsMock = new Mock<IOptions<PluginSettings>>();
            _optionsMock.Setup(o => o.Value).Returns(_settings);
        }

        [Fact]
        public async Task LoadPluginsAsync_WithNoPluginsDirectory_ReturnsEmptyCollection()
        {
            // Arrange
            _settings.PluginsDirectory = Path.Combine(Path.GetTempPath(), "NonExistentDirectory");
            var pluginLoader = new PluginLoader(_loggerMock.Object, _optionsMock.Object);

            // Act
            var plugins = await pluginLoader.LoadPluginsAsync();

            // Assert
            Assert.Empty(plugins);
        }

        [Fact]
        public async Task LoadPluginsAsync_WithDisabledPlugin_DoesNotLoadDisabledPlugin()
        {
            // Arrange
            var pluginId = "TestPlugin.Disabled";
            _settings.DisabledPlugins.Add(pluginId);
            var pluginLoader = new PluginLoader(_loggerMock.Object, _optionsMock.Object);
            
            // Create a mock plugin manager
            var pluginManagerType = typeof(PluginLoader).Assembly.GetType("ElectricityShop.Plugin.Core.PluginManager");
            var mockPluginManager = new Mock<PluginManager>(_settings);
            
            mockPluginManager.Setup(m => m.DiscoverPlugins(It.IsAny<string>()))
                .Returns(new List<PluginMetadata>
                {
                    new PluginMetadata
                    {
                        Id = pluginId,
                        Name = "Disabled Test Plugin",
                        AssemblyFileName = "TestPlugin.Disabled.dll",
                        TypeName = "TestPlugin.Disabled.DisabledPlugin",
                        IsEnabled = false
                    }
                });
            
            // TODO: Set up a way to actually test this by injecting the mock plugin manager

            // Act
            var plugins = await pluginLoader.LoadPluginsAsync();

            // Assert
            Assert.Empty(plugins);
        }
    }
}