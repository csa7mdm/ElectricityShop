using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.API.Controllers;
using ElectricityShop.Plugin.Abstractions.ExtensionPoints;
using ElectricityShop.Plugin.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ElectricityShop.API.Tests.Controllers
{
    public class RecommendationControllerTests
    {
        private readonly Mock<IPluginExtensionExecutor<IRecommendationExtensionPoint>> _executorMock;
        private readonly Mock<ILogger<RecommendationController>> _loggerMock;
        private readonly RecommendationController _controller;

        public RecommendationControllerTests()
        {
            _executorMock = new Mock<IPluginExtensionExecutor<IRecommendationExtensionPoint>>();
            _loggerMock = new Mock<ILogger<RecommendationController>>();
            _controller = new RecommendationController(_executorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetProductRecommendations_WithNoActivePlugins_ReturnsEmptyList()
        {
            // Arrange
            int productId = 1;
            int maxRecommendations = 5;
            
            _executorMock.Setup(e => e.ExecuteAllAsync(
                    It.IsAny<System.Func<IRecommendationExtensionPoint, Task<IEnumerable<int>>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<IEnumerable<int>>());
            
            // Act
            var result = await _controller.GetProductRecommendations(productId, maxRecommendations);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var recommendations = Assert.IsAssignableFrom<IEnumerable<int>>(okResult.Value);
            Assert.Empty(recommendations);
        }

        [Fact]
        public async Task GetProductRecommendations_WithSinglePlugin_ReturnsRecommendations()
        {
            // Arrange
            int productId = 1;
            int maxRecommendations = 5;
            var expectedRecommendations = new List<int> { 2, 3, 4, 5, 6 };
            
            _executorMock.Setup(e => e.ExecuteAllAsync(
                    It.IsAny<System.Func<IRecommendationExtensionPoint, Task<IEnumerable<int>>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<IEnumerable<int>> { expectedRecommendations });
            
            // Act
            var result = await _controller.GetProductRecommendations(productId, maxRecommendations);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var recommendations = Assert.IsAssignableFrom<IEnumerable<int>>(okResult.Value);
            Assert.Equal(expectedRecommendations.Count, recommendations.Count());
            Assert.Equal(expectedRecommendations, recommendations);
        }

        [Fact]
        public async Task GetProductRecommendations_WithMultiplePlugins_ReturnsCombinedRecommendations()
        {
            // Arrange
            int productId = 1;
            int maxRecommendations = 5;
            var plugin1Recommendations = new List<int> { 2, 3, 4 };
            var plugin2Recommendations = new List<int> { 3, 5, 6 }; // Note: 3 is in both lists
            
            _executorMock.Setup(e => e.ExecuteAllAsync(
                    It.IsAny<System.Func<IRecommendationExtensionPoint, Task<IEnumerable<int>>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<IEnumerable<int>> { plugin1Recommendations, plugin2Recommendations });
            
            // Act
            var result = await _controller.GetProductRecommendations(productId, maxRecommendations);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var recommendations = Assert.IsAssignableFrom<IEnumerable<int>>(okResult.Value);
            
            // Should have 5 unique recommendations (not 6, as 3 appears in both lists)
            Assert.Equal(5, recommendations.Count());
            
            // Should contain all unique recommendations
            Assert.Contains(2, recommendations);
            Assert.Contains(3, recommendations);
            Assert.Contains(4, recommendations);
            Assert.Contains(5, recommendations);
            Assert.Contains(6, recommendations);
        }

        [Fact]
        public async Task GetUserRecommendations_WithValidUserId_ReturnsRecommendations()
        {
            // Arrange
            int userId = 10;
            int maxRecommendations = 5;
            var expectedRecommendations = new List<int> { 1, 2, 3, 4, 5 };
            
            _executorMock.Setup(e => e.ExecuteAllAsync(
                    It.IsAny<System.Func<IRecommendationExtensionPoint, Task<IEnumerable<int>>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<IEnumerable<int>> { expectedRecommendations });
            
            // Act
            var result = await _controller.GetUserRecommendations(userId, maxRecommendations);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var recommendations = Assert.IsAssignableFrom<IEnumerable<int>>(okResult.Value);
            Assert.Equal(expectedRecommendations.Count, recommendations.Count());
            Assert.Equal(expectedRecommendations, recommendations);
        }
    }
}