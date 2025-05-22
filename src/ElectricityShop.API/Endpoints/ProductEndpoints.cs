using ElectricityShop.API.Models;
using ElectricityShop.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricityShop.API.Endpoints
{
    /// <summary>
    /// Product-related endpoints implemented as minimal APIs
    /// </summary>
    public static class ProductEndpoints
    {
        /// <summary>
        /// Maps product-related endpoints using minimal API syntax
        /// </summary>
        public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            // Search products endpoint - more efficient minimal API implementation
            app.MapGet("/api/products/search", async (
                string? searchTerm,
                decimal? minPrice,
                decimal? maxPrice,
                Guid? categoryId,
                int pageNumber = 1,
                int pageSize = 10,
                IMediator mediator = null!,
                IMemoryCache cache = null!) =>
            {
                // Create cache key based on parameters
                string cacheKey = $"products_search_{searchTerm}_{minPrice}_{maxPrice}_{categoryId}_{pageNumber}_{pageSize}";
                
                // Try to get from cache
                if (cache.TryGetValue(cacheKey, out PagedResponse<ProductDto> cachedResult))
                {
                    return Results.Ok(cachedResult);
                }
                
                // Create query
                var query = new GetProductsQuery
                {
                    SearchTerm = searchTerm,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    CategoryId = categoryId,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
                
                // Get results from mediator
                var products = await mediator.Send(query);
                
                // Create paged response
                var result = new PagedResponse<ProductDto>(
                    products,
                    query.TotalCount,
                    query.PageNumber,
                    query.PageSize);
                
                // Cache for 1 minute
                cache.Set(cacheKey, result, TimeSpan.FromMinutes(1));
                
                return Results.Ok(result);
            })
            .WithName("SearchProducts")
            .WithTags("Products")
            .WithOpenApi(operation => 
            {
                operation.Summary = "Search products";
                operation.Description = "Search products with filtering and pagination";
                return operation;
            });
            
            // Count products by category endpoint - simple aggregation example
            app.MapGet("/api/products/count-by-category", async (IMediator mediator) =>
            {
                var products = await mediator.Send(new GetProductsQuery { PageSize = int.MaxValue });
                
                var categoryCounts = products
                    .GroupBy(p => p.CategoryId)
                    .Select(g => new 
                    { 
                        CategoryId = g.Key, 
                        CategoryName = g.First().CategoryName,
                        Count = g.Count() 
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();
                    
                return Results.Ok(categoryCounts);
            })
            .WithName("CountProductsByCategory")
            .WithTags("Products")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Count products by category";
                operation.Description = "Returns the count of products in each category";
                return operation;
            });
            
            return app;
        }
    }
}