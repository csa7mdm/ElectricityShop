using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.Plugin.ProductRecommendation.Services
{
    /// <summary>
    /// Interface for product recommendation services.
    /// </summary>
    public interface IRecommendationService
    {
        /// <summary>
        /// Gets product recommendations based on similar products.
        /// </summary>
        /// <param name="productId">The ID of the product to find similar products for.</param>
        /// <param name="maxRecommendations">The maximum number of recommendations to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of recommended product IDs.</returns>
        Task<IEnumerable<int>> GetSimilarProductsAsync(
            int productId, 
            int maxRecommendations, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets product recommendations based on user behavior.
        /// </summary>
        /// <param name="userId">The ID of the user to get recommendations for.</param>
        /// <param name="maxRecommendations">The maximum number of recommendations to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of recommended product IDs.</returns>
        Task<IEnumerable<int>> GetUserBasedRecommendationsAsync(
            int userId, 
            int maxRecommendations, 
            CancellationToken cancellationToken = default);
    }
}