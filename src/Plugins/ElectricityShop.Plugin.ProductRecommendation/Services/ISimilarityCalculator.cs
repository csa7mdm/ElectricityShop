using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.Plugin.ProductRecommendation.Services
{
    /// <summary>
    /// Interface for calculating similarity between products.
    /// </summary>
    public interface ISimilarityCalculator
    {
        /// <summary>
        /// Calculates the similarity score between two products.
        /// </summary>
        /// <param name="productId1">The ID of the first product.</param>
        /// <param name="productId2">The ID of the second product.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A similarity score between 0 and 1, where 1 indicates identical products.</returns>
        Task<double> CalculateSimilarityAsync(
            int productId1, 
            int productId2, 
            CancellationToken cancellationToken = default);
    }
}