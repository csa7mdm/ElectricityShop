namespace ElectricityShop.Plugin.ProductRecommendation
{
    /// <summary>
    /// Settings for the recommendation system.
    /// </summary>
    public class RecommendationSettings
    {
        /// <summary>
        /// Gets or sets the maximum number of recommendations to return.
        /// </summary>
        public int MaxRecommendations { get; set; } = 5;

        /// <summary>
        /// Gets or sets a value indicating whether user-based recommendations are enabled.
        /// </summary>
        public bool EnableUserBasedRecommendations { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether product-based recommendations are enabled.
        /// </summary>
        public bool EnableProductBasedRecommendations { get; set; } = true;

        /// <summary>
        /// Gets or sets the minimum similarity score required for product recommendations.
        /// </summary>
        public double MinimumSimilarityScore { get; set; } = 0.5;
    }
}