using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.Plugin.ProductRecommendation.Services
{
    /// <summary>
    /// Interface for analyzing user behavior for product recommendations.
    /// </summary>
    public interface IUserBehaviorAnalyzer
    {
        /// <summary>
        /// Gets a list of product IDs that the user has interacted with.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of product IDs that the user has interacted with.</returns>
        Task<IEnumerable<int>> GetUserInteractedProductsAsync(
            int userId, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a list of product IDs that might interest the user based on their behavior.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="maxRecommendations">The maximum number of recommendations to return.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of product IDs that might interest the user.</returns>
        Task<IEnumerable<int>> GetRecommendedProductsAsync(
            int userId, 
            int maxRecommendations, 
            CancellationToken cancellationToken = default);
    }
}