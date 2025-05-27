using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Plugin.Abstractions.ExtensionPoints;
using ElectricityShop.Plugin.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ElectricityShop.API.Tests.Plugin
{
    public class PluginExtensionExecutorTests
    {
        public interface ITestExtensionPoint
        {
            int ExecutionOrder { get; }
            Task<string> GetValueAsync();
        }

        [Fact]
        public async Task ExecuteAllAsync_WithMultipleExtensions_ExecutesAllExtensions()
        {
            // Arrange
            var serviceProviderMock = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<PluginExtensionExecutor<ITestExtensionPoint>>>();

            var extension1Mock = new Mock<ITestExtensionPoint>();
            var extension2Mock = new Mock<ITestExtensionPoint>();

            extension1Mock.Setup(e => e.ExecutionOrder).Returns(1);
            extension1Mock.Setup(e => e.GetValueAsync()).ReturnsAsync("Value1");

            extension2Mock.Setup(e => e.ExecutionOrder).Returns(2);
            extension2Mock.Setup(e => e.GetValueAsync()).ReturnsAsync("Value2");

            var extensions = new List<ITestExtensionPoint> { extension1Mock.Object, extension2Mock.Object };

            serviceProviderMock.Setup(sp => sp.GetService(typeof(IEnumerable<ITestExtensionPoint>)))
                .Returns(extensions);

            var executor = new PluginExtensionExecutor<ITestExtensionPoint>(serviceProviderMock.Object, loggerMock.Object);

            // Act
            var results = await executor.ExecuteAllAsync(e => e.GetValueAsync());

            // Assert
            Assert.Equal(2, results.Count());
            Assert.Contains("Value1", results);
            Assert.Contains("Value2", results);
        }

        [Fact]
        public async Task ExecuteAllAsync_WithExceptionInExtension_ContinuesWithOtherExtensions()
        {
            // Arrange
            var serviceProviderMock = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<PluginExtensionExecutor<ITestExtensionPoint>>>();

            var extension1Mock = new Mock<ITestExtensionPoint>();
            var extension2Mock = new Mock<ITestExtensionPoint>();

            extension1Mock.Setup(e => e.ExecutionOrder).Returns(1);
            extension1Mock.Setup(e => e.GetValueAsync()).ThrowsAsync(new Exception("Test exception"));

            extension2Mock.Setup(e => e.ExecutionOrder).Returns(2);
            extension2Mock.Setup(e => e.GetValueAsync()).ReturnsAsync("Value2");

            var extensions = new List<ITestExtensionPoint> { extension1Mock.Object, extension2Mock.Object };

            serviceProviderMock.Setup(sp => sp.GetService(typeof(IEnumerable<ITestExtensionPoint>)))
                .Returns(extensions);

            var executor = new PluginExtensionExecutor<ITestExtensionPoint>(serviceProviderMock.Object, loggerMock.Object);

            // Act
            var results = await executor.ExecuteAllAsync(e => e.GetValueAsync());

            // Assert
            Assert.Single(results);
            Assert.Contains("Value2", results);
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Test exception")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteAllAsync_WithNoExtensions_ReturnsEmptyCollection()
        {
            // Arrange
            var serviceProviderMock = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<PluginExtensionExecutor<ITestExtensionPoint>>>();

            var extensions = Enumerable.Empty<ITestExtensionPoint>();

            serviceProviderMock.Setup(sp => sp.GetService(typeof(IEnumerable<ITestExtensionPoint>)))
                .Returns(extensions);

            var executor = new PluginExtensionExecutor<ITestExtensionPoint>(serviceProviderMock.Object, loggerMock.Object);

            // Act
            var results = await executor.ExecuteAllAsync(e => e.GetValueAsync());

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public async Task ExecuteAllAsync_WithOrderedExtensions_ExecutesInCorrectOrder()
        {
            // Arrange
            var serviceProviderMock = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<PluginExtensionExecutor<ITestExtensionPoint>>>();

            var extension1Mock = new Mock<ITestExtensionPoint>();
            var extension2Mock = new Mock<ITestExtensionPoint>();
            var extension3Mock = new Mock<ITestExtensionPoint>();

            extension1Mock.Setup(e => e.ExecutionOrder).Returns(3); // Highest order executes last
            extension2Mock.Setup(e => e.ExecutionOrder).Returns(1); // Lowest order executes first
            extension3Mock.Setup(e => e.ExecutionOrder).Returns(2); // Middle order executes second

            var executionOrder = new List<string>();

            extension1Mock.Setup(e => e.GetValueAsync()).Returns(() =>
            {
                executionOrder.Add("extension1");
                return Task.FromResult("Value1");
            });

            extension2Mock.Setup(e => e.GetValueAsync()).Returns(() =>
            {
                executionOrder.Add("extension2");
                return Task.FromResult("Value2");
            });

            extension3Mock.Setup(e => e.GetValueAsync()).Returns(() =>
            {
                executionOrder.Add("extension3");
                return Task.FromResult("Value3");
            });

            // Order is intentionally different from execution order
            var extensions = new List<ITestExtensionPoint> 
            { 
                extension1Mock.Object, 
                extension2Mock.Object, 
                extension3Mock.Object 
            };

            serviceProviderMock.Setup(sp => sp.GetService(typeof(IEnumerable<ITestExtensionPoint>)))
                .Returns(extensions);

            var executor = new PluginExtensionExecutor<ITestExtensionPoint>(serviceProviderMock.Object, loggerMock.Object);

            // Act
            var results = await executor.ExecuteAllAsync(e => e.GetValueAsync());

            // Assert
            Assert.Equal(3, results.Count());
            Assert.Equal("extension2", executionOrder[0]); // ExecutionOrder = 1
            Assert.Equal("extension3", executionOrder[1]); // ExecutionOrder = 2
            Assert.Equal("extension1", executionOrder[2]); // ExecutionOrder = 3
        }
    }
}