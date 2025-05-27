using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Plugin.Abstractions.ExtensionPoints;
using ElectricityShop.Plugin.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly IPluginExtensionExecutor<IRecommendationExtensionPoint> _recommendationExecutor;
        private readonly ILogger<RecommendationController> _logger;

        public RecommendationController(
            IPluginExtensionExecutor<IRecommendationExtensionPoint> recommendationExecutor,
            ILogger<RecommendationController> logger)
        {
            _recommendationExecutor = recommendationExecutor;
            _logger = logger;
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<int>>> GetProductRecommendations(
            int productId, 
            [FromQuery] int maxRecommendations = 5,
            CancellationToken cancellationToken = default)
        {
            var context = new Dictionary<string, object>();
            
            var allRecommendations = await _recommendationExecutor.ExecuteAllAsync(
                async extension => await extension.GetRecommendedProductsAsync(productId, maxRecommendations, context, cancellationToken),
                cancellationToken);

            var recommendations = allRecommendations
                .SelectMany(r => r)
                .Distinct()
                .Take(maxRecommendations)
                .ToList();

            return Ok(recommendations);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<int>>> GetUserRecommendations(
            int userId, 
            [FromQuery] int maxRecommendations = 5,
            CancellationToken cancellationToken = default)
        {
            var context = new Dictionary<string, object>();

            var allRecommendations = await _recommendationExecutor.ExecuteAllAsync(
                async extension => await extension.GetUserBasedRecommendationsAsync(userId, maxRecommendations, context, cancellationToken),
                cancellationToken);

            var recommendations = allRecommendations
                .SelectMany(r => r)
                .Distinct()
                .Take(maxRecommendations)
                .ToList();

            return Ok(recommendations);
        }
    }
}