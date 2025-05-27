using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ElectricityShop.API.Infrastructure.Middleware
{
    /// <summary>
    /// Middleware for adding cache control headers to responses.
    /// </summary>
    public class CacheControlMiddleware
    {
        private readonly RequestDelegate _next;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheControlMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public CacheControlMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            // Call next middleware
            await _next(context);
            
            // Add cache control headers for GET requests to product endpoints
            if (context.Request.Method == "GET" && 
                context.Request.Path.StartsWithSegments("/api/products"))
            {
                // If it's not already set by response caching middleware
                if (!context.Response.Headers.ContainsKey("Cache-Control"))
                {
                    // Public cache for CDNs and proxies
                    context.Response.Headers.Add("Cache-Control", "public, max-age=60");
                }
            }
        }
    }
    
    /// <summary>
    /// Extension methods for the cache control middleware.
    /// </summary>
    public static class CacheControlMiddlewareExtensions
    {
        /// <summary>
        /// Adds the cache control middleware to the application pipeline.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseCacheControlHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CacheControlMiddleware>();
        }
    }
}