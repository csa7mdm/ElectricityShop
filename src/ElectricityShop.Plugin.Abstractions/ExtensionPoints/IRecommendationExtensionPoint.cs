using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.Plugin.Abstractions.ExtensionPoints
{
    /// <summary>
    /// Represents an extension point for product recommendation operations.
    /// </summary>
    public interface IRecommendationExtensionPoint
    {
        /// <summary>
        /// Gets the execution order of this extension point. Lower values execute earlier.
        /// </summary>
        int ExecutionOrder { get; }

        /// <summary>
        /// Gets the name of this recommendation algorithm.
        /// </summary>
        string RecommendationAlgorithmName { get; }

        /// <summary>
        /// Provides recommended product IDs based on a specific product.
        /// </summary>
        /// <param name="productId">The ID of the product to base recommendations on.</param>
        /// <param name="maxRecommendations">The maximum number of recommendations to return.</param>
        /// <param name="context">Additional context data for the recommendation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of recommended product IDs.</returns>
        Task<IEnumerable<int>> GetRecommendedProductsAsync(
            int productId, 
            int maxRecommendations, 
            IDictionary<string, object> context, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Provides recommended product IDs based on a user's history.
        /// </summary>
        /// <param name="userId">The ID of the user to get recommendations for.</param>
        /// <param name="maxRecommendations">The maximum number of recommendations to return.</param>
        /// <param name="context">Additional context data for the recommendation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of recommended product IDs.</returns>
        Task<IEnumerable<int>> GetUserBasedRecommendationsAsync(
            int userId, 
            int maxRecommendations, 
            IDictionary<string, object> context, 
            CancellationToken cancellationToken = default);
    }
}