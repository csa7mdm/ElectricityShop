using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Plugin.Abstractions.ExtensionPoints;
using ElectricityShop.Plugin.ProductRecommendation.Services;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.Plugin.ProductRecommendation
{
    /// <summary>
    /// Provides product recommendations through the recommendation extension point.
    /// </summary>
    public class ProductRecommendationExtensionPoint : IRecommendationExtensionPoint
    {
        private readonly IRecommendationService _recommendationService;
        private readonly ILogger<ProductRecommendationExtensionPoint> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRecommendationExtensionPoint"/> class.
        /// </summary>
        /// <param name="recommendationService">The recommendation service.</param>
        /// <param name="logger">The logger.</param>
        public ProductRecommendationExtensionPoint(
            IRecommendationService recommendationService,
            ILogger<ProductRecommendationExtensionPoint> logger)
        {
            _recommendationService = recommendationService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the execution order of this extension point. Lower values execute earlier.
        /// </summary>
        public int ExecutionOrder => 100;

        /// <summary>
        /// Gets the name of this recommendation algorithm.
        /// </summary>
        public string RecommendationAlgorithmName => "Hybrid Recommendation Algorithm";

        /// <summary>
        /// Provides recommended product IDs based on a specific product.
        /// </summary>
        /// <param name="productId">The ID of the product to base recommendations on.</param>
        /// <param name="maxRecommendations">The maximum number of recommendations to return.</param>
        /// <param name="context">Additional context data for the recommendation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of recommended product IDs.</returns>
        public async Task<IEnumerable<int>> GetRecommendedProductsAsync(
            int productId, 
            int maxRecommendations, 
            IDictionary<string, object> context, 
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting product-based recommendations for product {ProductId}", productId);
            
            // Extract user ID from context if present
            int? userId = null;
            if (context.TryGetValue("UserId", out var userIdObj) && userIdObj is int id)
            {
                userId = id;
            }

            // Get product-based recommendations
            var recommendations = await _recommendationService.GetSimilarProductsAsync(
                productId, 
                maxRecommendations, 
                cancellationToken);
            
            // If we have a user ID, enhance with user-based recommendations
            if (userId.HasValue)
            {
                var userRecommendations = await _recommendationService.GetUserBasedRecommendationsAsync(
                    userId.Value, 
                    maxRecommendations, 
                    cancellationToken);
                
                // Combine and deduplicate recommendations
                recommendations = recommendations
                    .Union(userRecommendations)
                    .Distinct()
                    .Take(maxRecommendations);
            }
            
            return recommendations;
        }

        /// <summary>
        /// Provides recommended product IDs based on a user's history.
        /// </summary>
        /// <param name="userId">The ID of the user to get recommendations for.</param>
        /// <param name="maxRecommendations">The maximum number of recommendations to return.</param>
        /// <param name="context">Additional context data for the recommendation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of recommended product IDs.</returns>
        public async Task<IEnumerable<int>> GetUserBasedRecommendationsAsync(
            int userId, 
            int maxRecommendations, 
            IDictionary<string, object> context, 
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting user-based recommendations for user {UserId}", userId);
            
            return await _recommendationService.GetUserBasedRecommendationsAsync(
                userId, 
                maxRecommendations, 
                cancellationToken);
        }
    }
}