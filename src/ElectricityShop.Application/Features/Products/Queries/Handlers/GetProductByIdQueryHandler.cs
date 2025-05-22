using ElectricityShop.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Features.Products.Queries.Handlers
{
    /// <summary>
    /// Handler for retrieving a single product by ID
    /// </summary>
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICacheService _cacheService;
        
        /// <summary>
        /// Initializes a new instance of GetProductByIdQueryHandler
        /// </summary>
        /// <param name="dbContext">Application DB context</param>
        /// <param name="cacheService">Cache service for optimized retrieval</param>
        public GetProductByIdQueryHandler(
            IApplicationDbContext dbContext,
            ICacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
        }
        
        /// <summary>
        /// Handles the query, retrieving from cache if available
        /// </summary>
        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            // Create cache key for this product
            string cacheKey = $"product:{request.Id}";
            
            // Try to get the product from cache first
            var cachedProduct = await _cacheService.GetAsync<ProductDto>(cacheKey);
            if (cachedProduct != null)
            {
                return cachedProduct;
            }
            
            // If not in cache, get from database
            var product = await (from p in _dbContext.Products
                          join c in _dbContext.Categories on p.CategoryId equals c.Id
                          where p.Id == request.Id
                          select new ProductDto
                          {
                              Id = p.Id,
                              Name = p.Name,
                              Description = p.Description,
                              Price = p.Price,
                              StockQuantity = p.StockQuantity,
                              CategoryId = p.CategoryId,
                              CategoryName = c.Name,
                              IsActive = p.IsActive
                              // Other properties mapped here...
                          }).FirstOrDefaultAsync(cancellationToken);
            
            // If product was found, cache it
            if (product != null)
            {
                // Cache the product for 30 minutes
                await _cacheService.SetAsync(cacheKey, product, TimeSpan.FromMinutes(30));
            }
            
            return product;
        }
    }
}