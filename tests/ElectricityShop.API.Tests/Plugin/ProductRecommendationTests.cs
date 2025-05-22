using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Interfaces;
using ElectricityShop.Plugin.Abstractions.ExtensionPoints;
using ElectricityShop.Plugin.ProductRecommendation;
using ElectricityShop.Plugin.ProductRecommendation.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ElectricityShop.API.Tests.Plugin
{
    public class ProductRecommendationTests
    {
        private readonly Mock<IRecommendationService> _recommendationServiceMock;
        private readonly Mock<ILogger<ProductRecommendationExtensionPoint>> _loggerMock;

        public ProductRecommendationTests()
        {
            _recommendationServiceMock = new Mock<IRecommendationService>();
            _loggerMock = new Mock<ILogger<ProductRecommendationExtensionPoint>>();
        }

        [Fact]
        public async Task GetRecommendedProductsAsync_WithValidProductId_ReturnsRecommendations()
        {
            // Arrange
            int productId = 1;
            int maxRecommendations = 5;
            var context = new Dictionary<string, object>();
            var expectedRecommendations = new List<int> { 2, 3, 4, 5, 6 };

            _recommendationServiceMock.Setup(s => s.GetSimilarProductsAsync(
                    productId, maxRecommendations, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRecommendations);

            var extensionPoint = new ProductRecommendationExtensionPoint(
                _recommendationServiceMock.Object,
                _loggerMock.Object);

            // Act
            var result = await extensionPoint.GetRecommendedProductsAsync(
                productId, maxRecommendations, context);

            // Assert
            Assert.Equal(expectedRecommendations.Count, result.Count());
            Assert.Equal(expectedRecommendations, result);
        }

        [Fact]
        public async Task GetRecommendedProductsAsync_WithUserIdInContext_CombinesUserBasedRecommendations()
        {
            // Arrange
            int productId = 1;
            int userId = 10;
            int maxRecommendations = 5;
            var context = new Dictionary<string, object> { { "UserId", userId } };
            
            var productBasedRecommendations = new List<int> { 2, 3, 4 };
            var userBasedRecommendations = new List<int> { 3, 5, 6 }; // Note: 3 is in both lists
            var expectedCombinedRecommendations = new List<int> { 2, 3, 4, 5, 6 };

            _recommendationServiceMock.Setup(s => s.GetSimilarProductsAsync(
                    productId, maxRecommendations, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productBasedRecommendations);

            _recommendationServiceMock.Setup(s => s.GetUserBasedRecommendationsAsync(
                    userId, maxRecommendations, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userBasedRecommendations);

            var extensionPoint = new ProductRecommendationExtensionPoint(
                _recommendationServiceMock.Object,
                _loggerMock.Object);

            // Act
            var result = await extensionPoint.GetRecommendedProductsAsync(
                productId, maxRecommendations, context);

            // Assert
            Assert.Equal(5, result.Count()); // 5 unique recommendations (not 6, as 3 appears in both lists)
            Assert.Equal(expectedCombinedRecommendations.OrderBy(x => x), result.OrderBy(x => x));
        }

        [Fact]
        public async Task GetUserBasedRecommendationsAsync_WithValidUserId_ReturnsRecommendations()
        {
            // Arrange
            int userId = 10;
            int maxRecommendations = 5;
            var context = new Dictionary<string, object>();
            var expectedRecommendations = new List<int> { 1, 2, 3, 4, 5 };

            _recommendationServiceMock.Setup(s => s.GetUserBasedRecommendationsAsync(
                    userId, maxRecommendations, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRecommendations);

            var extensionPoint = new ProductRecommendationExtensionPoint(
                _recommendationServiceMock.Object,
                _loggerMock.Object);

            // Act
            var result = await extensionPoint.GetUserBasedRecommendationsAsync(
                userId, maxRecommendations, context);

            // Assert
            Assert.Equal(expectedRecommendations.Count, result.Count());
            Assert.Equal(expectedRecommendations, result);
        }
    }
    
    public class RecommendationServiceTests
    {
        private readonly Mock<ISimilarityCalculator> _similarityCalculatorMock;
        private readonly Mock<IUserBehaviorAnalyzer> _userBehaviorAnalyzerMock;
        private readonly Mock<IRepository<Product>> _productRepositoryMock;
        private readonly Mock<ILogger<RecommendationService>> _loggerMock;
        private readonly RecommendationSettings _settings;
        private readonly Mock<IOptions<RecommendationSettings>> _optionsMock;

        public RecommendationServiceTests()
        {
            _similarityCalculatorMock = new Mock<ISimilarityCalculator>();
            _userBehaviorAnalyzerMock = new Mock<IUserBehaviorAnalyzer>();
            _productRepositoryMock = new Mock<IRepository<Product>>();
            _loggerMock = new Mock<ILogger<RecommendationService>>();
            
            _settings = new RecommendationSettings
            {
                MaxRecommendations = 5,
                EnableProductBasedRecommendations = true,
                EnableUserBasedRecommendations = true,
                MinimumSimilarityScore = 0.5
            };
            
            _optionsMock = new Mock<IOptions<RecommendationSettings>>();
            _optionsMock.Setup(o => o.Value).Returns(_settings);
        }

        [Fact]
        public async Task GetSimilarProductsAsync_WithValidProductId_ReturnsSimilarProducts()
        {
            // Arrange
            int productId = 1;
            int maxRecommendations = 3;
            
            // Products 2 and 3 are similar to product 1, product 4 is not similar enough
            var allProductIds = new List<int> { 1, 2, 3, 4 };
            var queryable = allProductIds.AsQueryable();
            
            _productRepositoryMock.Setup(r => r.GetAllAsQueryable())
                .Returns(queryable);
            
            _similarityCalculatorMock.Setup(c => c.CalculateSimilarityAsync(
                    productId, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(0.8); // High similarity
                
            _similarityCalculatorMock.Setup(c => c.CalculateSimilarityAsync(
                    productId, 3, It.IsAny<CancellationToken>()))
                .ReturnsAsync(0.7); // Medium similarity
                
            _similarityCalculatorMock.Setup(c => c.CalculateSimilarityAsync(
                    productId, 4, It.IsAny<CancellationToken>()))
                .ReturnsAsync(0.3); // Low similarity, below threshold
            
            var service = new RecommendationService(
                _similarityCalculatorMock.Object,
                _userBehaviorAnalyzerMock.Object,
                _productRepositoryMock.Object,
                _optionsMock.Object,
                _loggerMock.Object);
                
            // Act
            var result = await service.GetSimilarProductsAsync(
                productId, maxRecommendations);
                
            // Assert
            Assert.Equal(2, result.Count()); // Only products 2 and 3 are similar enough
            Assert.Contains(2, result); // Product 2 has highest similarity
            Assert.Contains(3, result); // Product 3 has medium similarity
            Assert.DoesNotContain(4, result); // Product 4 is below similarity threshold
        }

        [Fact]
        public async Task GetUserBasedRecommendationsAsync_WithValidUserId_ReturnsUserBasedRecommendations()
        {
            // Arrange
            int userId = 10;
            int maxRecommendations = 5;
            var expectedRecommendations = new List<int> { 1, 2, 3, 4, 5 };
            
            _userBehaviorAnalyzerMock.Setup(a => a.GetRecommendedProductsAsync(
                    userId, maxRecommendations, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRecommendations);
                
            var service = new RecommendationService(
                _similarityCalculatorMock.Object,
                _userBehaviorAnalyzerMock.Object,
                _productRepositoryMock.Object,
                _optionsMock.Object,
                _loggerMock.Object);
                
            // Act
            var result = await service.GetUserBasedRecommendationsAsync(
                userId, maxRecommendations);
                
            // Assert
            Assert.Equal(expectedRecommendations.Count, result.Count());
            Assert.Equal(expectedRecommendations, result);
        }
    }
}