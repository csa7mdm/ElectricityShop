using ElectricityShop.API.Infrastructure.Telemetry;
using ElectricityShop.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ElectricityShop.API.Controllers
{
    /// <summary>
    /// Example controller showing telemetry implementation
    /// This is for demonstration purposes only and should not be used in production
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TelemetryExampleController : ControllerBase
    {
        private readonly ILogger<TelemetryExampleController> _controllerLogger;

        public TelemetryExampleController(ILogger<TelemetryExampleController> logger)
        {
            _controllerLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Simulates a product view operation with telemetry tracking
        /// </summary>
        [HttpGet("product/{id}")]
        public IActionResult GetProduct(int id)
        {
            _controllerLogger.LogInformation("Get product request for ID: {ProductId}", id);

            // Track product view metric
            ApiMetrics.ProductViews.Add(1);

            // Simulate processing time
            var stopwatch = Stopwatch.StartNew();
            Task.Delay(50).Wait(); // Simulating database lookup
            stopwatch.Stop();

            // Record time spent retrieving the product
            ApiMetrics.RequestDuration.Record(stopwatch.ElapsedMilliseconds);

            // Track user activity if authenticated
            if (User.Identity?.IsAuthenticated == true)
            {
                TelemetryService.RecordUserActivity(User.Identity.Name);
            }

            return Ok(new { Id = id, Name = $"Product {id}", Price = 19.99m * id });
        }

        /// <summary>
        /// Simulates product search with telemetry tracking
        /// </summary>
        [HttpGet("search")]
        public IActionResult SearchProducts([FromQuery] string query)
        {
            _controllerLogger.LogInformation("Search products request with query: {Query}", query);

            // Track product search metric
            ApiMetrics.ProductSearches.Add(1);

            // Simulate processing time with some randomness
            var stopwatch = Stopwatch.StartNew();
            var rng = new Random();
            Task.Delay(rng.Next(50, 200)).Wait(); // Simulating search operation
            stopwatch.Stop();

            // Record response time
            ApiMetrics.RequestDuration.Record(stopwatch.ElapsedMilliseconds);

            // Track user activity if authenticated
            if (User.Identity?.IsAuthenticated == true)
            {
                TelemetryService.RecordUserActivity(User.Identity.Name);
            }

            // Simulate cache hit or miss
            bool cacheHit = rng.Next(100) < 70; // 70% cache hit rate
            if (cacheHit)
            {
                ApiMetrics.CacheHits.Add(1);
                _controllerLogger.LogDebug("Cache hit for search: {Query}", query);
            }
            else
            {
                ApiMetrics.CacheMisses.Add(1);
                _controllerLogger.LogDebug("Cache miss for search: {Query}", query);
            }

            // Return some dummy results
            var results = new List<object>();
            var resultCount = rng.Next(1, 10);
            for (int i = 1; i <= resultCount; i++)
            {
                results.Add(new { Id = i, Name = $"Product {i} matching '{query}'", Price = 19.99m * i });
            }

            return Ok(results);
        }

        /// <summary>
        /// Simulates creating an order with telemetry tracking
        /// </summary>
        [HttpPost("order")]
        [Authorize]
        public IActionResult CreateOrder([FromBody] OrderRequest request)
        {
            _controllerLogger.LogInformation("Create order request received from user {UserId}", User.Identity.Name);

            // Track order creation metric
            ApiMetrics.OrdersCreated.Add(1);

            // Simulate processing
            var stopwatch = Stopwatch.StartNew();
            Task.Delay(100).Wait(); // Simulating database operation
            stopwatch.Stop();

            // Record response time
            ApiMetrics.RequestDuration.Record(stopwatch.ElapsedMilliseconds);

            // Track order amount for business metrics
            TelemetryService.RecordOrder(request.TotalAmount);

            // Track user activity
            TelemetryService.RecordUserActivity(User.Identity.Name);

            return Ok(new { OrderId = Guid.NewGuid(), Status = "Created", TotalAmount = request.TotalAmount });
        }

        /// <summary>
        /// Simulates a login attempt with telemetry tracking
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] TelemetryLoginRequest request)
        {
            _controllerLogger.LogInformation("Login attempt for user {Username}", request.Username);

            // Simulate successful or failed login (80% success rate for demo)
            var rng = new Random();
            bool loginSuccessful = rng.Next(100) < 80;

            if (loginSuccessful)
            {
                ApiMetrics.SuccessfulLogins.Add(1);
                _controllerLogger.LogInformation("Successful login for user {Username}", request.Username);
                return Ok(new { Token = "demo-token", Message = "Login successful" });
            }
            else
            {
                ApiMetrics.FailedLogins.Add(1);
                _controllerLogger.LogWarning("Failed login attempt for user {Username}", request.Username);
                return Unauthorized(new { Message = "Invalid credentials" });
            }
        }

        /// <summary>
        /// Returns telemetry statistics for testing
        /// </summary>
        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetTelemetryStats()
        {
            // Track user activity
            if (User.Identity?.IsAuthenticated == true)
            {
                TelemetryService.RecordUserActivity(User.Identity.Name);
            }

            return Ok(new
            {
                ActiveUsers = TelemetryService.GetActiveUsers(),
                AverageOrderValue = TelemetryService.GetAverageOrderValue(),
                ProductCount = TelemetryService.GetProductCount(),
                SystemMetrics = new
                {
                    CpuUsagePercent = TelemetryService.GetCpuUsage(),
                    MemoryUsageMB = TelemetryService.GetMemoryUsage()
                }
            });
        }
    }
}