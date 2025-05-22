using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Features.Products.Commands.Handlers
{
    /// <summary>
    /// Handler for updating a product with cache invalidation
    /// </summary>
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICacheInvalidationService _cacheInvalidation;
        
        /// <summary>
        /// Initializes a new instance of the UpdateProductCommandHandler
        /// </summary>
        /// <param name="dbContext">Application DB context</param>
        /// <param name="cacheInvalidation">Cache invalidation service</param>
        public UpdateProductCommandHandler(
            IApplicationDbContext dbContext,
            ICacheInvalidationService cacheInvalidation)
        {
            _dbContext = dbContext;
            _cacheInvalidation = cacheInvalidation;
        }
        
        /// <summary>
        /// Handles the update product command
        /// </summary>
        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            // Find the product
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
                
            if (product == null)
            {
                return false;
            }
            
            // Check if category is changing for cache invalidation
            bool categoryChanged = product.CategoryId != request.CategoryId;
            Guid oldCategoryId = product.CategoryId;
            
            // Update the product properties
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.CategoryId = request.CategoryId;
            product.IsActive = request.IsActive;
            product.UpdatedAt = DateTime.UtcNow;
            
            // Save changes
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            // Invalidate caches
            await _cacheInvalidation.InvalidateProductCacheAsync((int)product.Id);
            
            // If category changed, invalidate category caches
            if (categoryChanged)
            {
                await _cacheInvalidation.InvalidateProductsByCategoryAsync((int)oldCategoryId);
                await _cacheInvalidation.InvalidateProductsByCategoryAsync((int)request.CategoryId);
            }
            
            return true;
        }
    }
}