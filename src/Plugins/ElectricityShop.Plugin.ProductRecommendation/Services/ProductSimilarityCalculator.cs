using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.Plugin.ProductRecommendation.Services
{
    /// <summary>
    /// Calculates similarity between products based on their attributes.
    /// </summary>
    public class ProductSimilarityCalculator : ISimilarityCalculator
    {
        private readonly IRepository<Product> _productRepository;
        private readonly ILogger<ProductSimilarityCalculator> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductSimilarityCalculator"/> class.
        /// </summary>
        /// <param name="productRepository">The product repository.</param>
        /// <param name="logger">The logger.</param>
        public ProductSimilarityCalculator(
            IRepository<Product> productRepository,
            ILogger<ProductSimilarityCalculator> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        /// <summary>
        /// Calculates the similarity score between two products.
        /// </summary>
        /// <param name="productId1">The ID of the first product.</param>
        /// <param name="productId2">The ID of the second product.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A similarity score between 0 and 1, where 1 indicates identical products.</returns>
        public async Task<double> CalculateSimilarityAsync(
            int productId1, 
            int productId2, 
            CancellationToken cancellationToken = default)
        {
            if (productId1 == productId2)
            {
                return 1.0; // Same product, perfect similarity
            }

            try
            {
                // Get products from repository
                var product1 = await _productRepository.GetByIdAsync(productId1);
                var product2 = await _productRepository.GetByIdAsync(productId2);

                if (product1 == null || product2 == null)
                {
                    _logger.LogWarning("Could not calculate similarity - one or both products not found: {ProductId1}, {ProductId2}", 
                        productId1, productId2);
                    return 0.0;
                }

                // Calculate similarity based on product attributes
                // This is a simplified example - in a real implementation, you would use more sophisticated algorithms
                double similarity = 0.0;

                // Add similarity based on category
                if (product1.CategoryId == product2.CategoryId)
                {
                    similarity += 0.3;
                }

                // Add similarity based on price range
                double priceDifference = Math.Abs(product1.Price - product2.Price);
                double priceMax = Math.Max(product1.Price, product2.Price);
                if (priceMax > 0)
                {
                    double priceRatio = 1.0 - (priceDifference / priceMax);
                    similarity += 0.3 * priceRatio;
                }

                // Add similarity based on brand
                if (product1.BrandId == product2.BrandId)
                {
                    similarity += 0.2;
                }

                // Add similarity based on other attributes (simplified)
                similarity += 0.2 * CalculateAttributeSimilarity(product1, product2);

                return Math.Min(similarity, 1.0); // Ensure similarity is at most 1.0
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating similarity between products {ProductId1} and {ProductId2}", 
                    productId1, productId2);
                return 0.0;
            }
        }

        private double CalculateAttributeSimilarity(Product product1, Product product2)
        {
            // This is a placeholder for attribute similarity calculation
            // In a real implementation, you would compare product attributes, features, specifications, etc.
            return 0.5;
        }
    }
}