using System;
using System.Threading.Tasks;
using ElectricityShop.Plugin.Abstractions;
using ElectricityShop.Plugin.Abstractions.ExtensionPoints;
using ElectricityShop.Plugin.ProductRecommendation.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.Plugin.ProductRecommendation
{
    /// <summary>
    /// Plugin that provides product recommendation functionality.
    /// </summary>
    public class ProductRecommendationPlugin : IPlugin
    {
        private readonly ILogger<ProductRecommendationPlugin> _logger;
        private IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRecommendationPlugin"/> class.
        /// </summary>
        public ProductRecommendationPlugin()
        {
            // Logger will be assigned during initialization
            _logger = null;
        }

        /// <summary>
        /// Gets the unique identifier for this plugin.
        /// </summary>
        public string Id => "ElectricityShop.Plugin.ProductRecommendation";

        /// <summary>
        /// Gets the name of this plugin.
        /// </summary>
        public string Name => "Product Recommendation Plugin";

        /// <summary>
        /// Gets the version of this plugin.
        /// </summary>
        public Version Version => new Version(1, 0, 0);

        /// <summary>
        /// Gets the author of this plugin.
        /// </summary>
        public string Author => "ElectricityShop";

        /// <summary>
        /// Gets a description of this plugin.
        /// </summary>
        public string Description => "Provides product recommendation functionality based on user behavior and product properties";

        /// <summary>
        /// Configures services for this plugin.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <returns>The configured service collection.</returns>
        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // Register plugin services
            services.AddScoped<ISimilarityCalculator, ProductSimilarityCalculator>();
            services.AddScoped<IUserBehaviorAnalyzer, UserBehaviorAnalyzer>();
            services.AddScoped<IRecommendationService, RecommendationService>();
            
            // Register extension points
            services.AddScoped<IRecommendationExtensionPoint, ProductRecommendationExtensionPoint>();
            
            return services;
        }

        /// <summary>
        /// Initializes this plugin.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task InitializeAsync(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetService<ILogger<ProductRecommendationPlugin>>();
            _logger?.LogInformation("Product Recommendation Plugin initialized");
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Shuts down this plugin.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task ShutdownAsync()
        {
            _logger?.LogInformation("Product Recommendation Plugin shut down");
            return Task.CompletedTask;
        }
    }
}