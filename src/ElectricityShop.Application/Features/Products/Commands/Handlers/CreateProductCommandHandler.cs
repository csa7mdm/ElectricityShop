using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Features.Products.Commands.Handlers
{
    /// <summary>
    /// Handler for creating a new product
    /// </summary>
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICacheInvalidationService _cacheInvalidation;
        
        /// <summary>
        /// Initializes a new instance of the CreateProductCommandHandler
        /// </summary>
        /// <param name="dbContext">Application DB context</param>
        /// <param name="cacheInvalidation">Cache invalidation service</param>
        public CreateProductCommandHandler(
            IApplicationDbContext dbContext,
            ICacheInvalidationService cacheInvalidation)
        {
            _dbContext = dbContext;
            _cacheInvalidation = cacheInvalidation;
        }
        
        /// <summary>
        /// Handles the create product command
        /// </summary>
        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Create a new product entity
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                CategoryId = request.CategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            // Add to database
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            // Invalidate the products cache for the category
            await _cacheInvalidation.InvalidateProductsByCategoryAsync((int)request.CategoryId);
            
            // Invalidate all products cache as a new product has been added
            await _cacheInvalidation.InvalidateAllProductsCacheAsync();
            
            return product.Id;
        }
    }
}