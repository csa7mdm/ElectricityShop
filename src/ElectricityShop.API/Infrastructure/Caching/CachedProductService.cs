using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectricityShop.API.Models;
using ElectricityShop.Application.Features.Products.Queries;
using Microsoft.Extensions.Caching.Memory;
using MediatR;
using System.Threading;

namespace ElectricityShop.API.Infrastructure.Caching
{
    /// <summary>
    /// Decorator for product queries that adds caching capabilities
    /// </summary>
    public class CachedProductService : IRequestHandler<GetProductsQuery, List<ProductDto>>,
                                      IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IMemoryCache _cache;
        private readonly IRequestHandler<GetProductsQuery, List<ProductDto>> _productsHandler;
        private readonly IRequestHandler<GetProductByIdQuery, ProductDto> _productByIdHandler;

        // Cache expiration times
        private static readonly TimeSpan ProductListCacheTime = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan ProductDetailsCacheTime = TimeSpan.FromMinutes(30);

        public CachedProductService(
            IMemoryCache cache,
            IRequestHandler<GetProductsQuery, List<ProductDto>> productsHandler,
            IRequestHandler<GetProductByIdQuery, ProductDto> productByIdHandler)
        {
            _cache = cache;
            _productsHandler = productsHandler;
            _productByIdHandler = productByIdHandler;
        }

        public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = CacheKeys.ProductsList(request.PageNumber, request.PageSize, request.CategoryId);

            // Try to get data from cache
            if (_cache.TryGetValue(cacheKey, out List<ProductDto> cachedProducts))
            {
                return cachedProducts;
            }

            // If not in cache, get from original handler
            var products = await _productsHandler.Handle(request, cancellationToken);

            // Cache the products
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(ProductListCacheTime);

            _cache.Set(cacheKey, products, cacheEntryOptions);

            return products;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = CacheKeys.ProductDetails(request.Id);

            // Try to get data from cache
            if (_cache.TryGetValue(cacheKey, out ProductDto cachedProduct))
            {
                return cachedProduct;
            }

            // If not in cache, get from original handler
            var product = await _productByIdHandler.Handle(request, cancellationToken);

            // Only cache if product exists
            if (product != null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(ProductDetailsCacheTime);

                _cache.Set(cacheKey, product, cacheEntryOptions);
            }

            return product;
        }
    }
}