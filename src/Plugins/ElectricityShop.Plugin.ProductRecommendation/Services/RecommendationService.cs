using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElectricityShop.Plugin.ProductRecommendation.Services
{
    /// <summary>
    /// Service that provides product recommendations.
    /// </summary>
    public class RecommendationService : IRecommendationService
    {
        private readonly ISimilarityCalculator _similarityCalculator;
        private readonly IUserBehaviorAnalyzer _userBehaviorAnalyzer;
        private readonly IRepository<Product> _productRepository;
        private readonly ILogger<RecommendationService> _logger;
        private readonly RecommendationSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecommendationService"/> class.
        /// </summary>
        /// <param name="similarityCalculator">The similarity calculator.</param>
        /// <param name="userBehaviorAnalyzer">The user behavior analyzer.</param>
        /// <param name="productRepository">The product repository.</param>
        /// <param name="options">The recommendation settings options.</param>
        /// <param name="logger">The logger.</param>
        public RecommendationService(
            ISimilarityCalculator similarityCalculator,
            IUserBehaviorAnalyzer userBehaviorAnalyzer,
            IRepository<Product> productRepository,
            IOptions<RecommendationSettings> options,
            ILogger<RecommendationService> logger)
        {
            _similarityCalculator = similarityCalculator;
            _userBehaviorAnalyzer = userBehaviorAnalyzer;
            _productRepository = productRepository;
            _logger = logger;
            _settings = options.Value;
        }

        /// <summary>
        /// Gets product recommendations based on similar products.
        /// </summary>
        /// <param name="productId">The ID of the product to find similar products for.</param>
        /// <param name="maxRecommendations">The maximum number of recommendations to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of recommended product IDs.</returns>
        public async Task<IEnumerable<int>> GetSimilarProductsAsync(
            int productId, 
            int maxRecommendations, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting similar products for product {ProductId}", productId);

                // Get all product IDs
                var allProductIds = await GetAllProductIdsAsync(cancellationToken);
                
                // Calculate similarity for each product
                var similarityScores = new Dictionary<int, double>();
                foreach (var candidateId in allProductIds)
                {
                    if (candidateId == productId)
                    {
                        continue; // Skip the source product
                    }

                    var similarity = await _similarityCalculator.CalculateSimilarityAsync(
                        productId, 
                        candidateId, 
                        cancellationToken);
                    
                    if (similarity >= _settings.MinimumSimilarityScore)
                    {
                        similarityScores[candidateId] = similarity;
                    }
                }

                // Sort by similarity score and take the top results
                var recommendations = similarityScores
                    .OrderByDescending(kv => kv.Value)
                    .Take(maxRecommendations)
                    .Select(kv => kv.Key)
                    .ToList();

                _logger.LogInformation("Found {Count} similar products for product {ProductId}", 
                    recommendations.Count, productId);
                
                return recommendations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting similar products for product {ProductId}", productId);
                return Enumerable.Empty<int>();
            }
        }

        /// <summary>
        /// Gets product recommendations based on user behavior.
        /// </summary>
        /// <param name="userId">The ID of the user to get recommendations for.</param>
        /// <param name="maxRecommendations">The maximum number of recommendations to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of recommended product IDs.</returns>
        public async Task<IEnumerable<int>> GetUserBasedRecommendationsAsync(
            int userId, 
            int maxRecommendations, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting user-based recommendations for user {UserId}", userId);

                // Get recommendations from the user behavior analyzer
                var recommendations = await _userBehaviorAnalyzer.GetRecommendedProductsAsync(
                    userId, 
                    maxRecommendations, 
                    cancellationToken);

                _logger.LogInformation("Found {Count} recommendations for user {UserId}", 
                    recommendations.Count(), userId);
                
                return recommendations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user-based recommendations for user {UserId}", userId);
                return Enumerable.Empty<int>();
            }
        }

        private async Task<IEnumerable<int>> GetAllProductIdsAsync(CancellationToken cancellationToken)
        {
            try
            {
                // In a real implementation, you would query the database for all product IDs
                // Here we're simulating it with a simplified implementation
                var query = _productRepository.GetAllAsQueryable();
                var productIds = query.Select(p => p.Id).ToList();
                
                return productIds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all product IDs");
                return Enumerable.Empty<int>();
            }
        }
    }
}