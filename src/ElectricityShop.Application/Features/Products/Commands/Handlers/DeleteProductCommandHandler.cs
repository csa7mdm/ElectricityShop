using ElectricityShop.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Features.Products.Commands.Handlers
{
    /// <summary>
    /// Handler for deleting a product
    /// </summary>
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICacheInvalidationService _cacheInvalidation;
        
        /// <summary>
        /// Initializes a new instance of the DeleteProductCommandHandler
        /// </summary>
        /// <param name="dbContext">Application DB context</param>
        /// <param name="cacheInvalidation">Cache invalidation service</param>
        public DeleteProductCommandHandler(
            IApplicationDbContext dbContext,
            ICacheInvalidationService cacheInvalidation)
        {
            _dbContext = dbContext;
            _cacheInvalidation = cacheInvalidation;
        }
        
        /// <summary>
        /// Handles the delete product command
        /// </summary>
        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            // Find the product
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
                
            if (product == null)
            {
                return false;
            }
            
            // Store category ID for cache invalidation
            var categoryId = product.CategoryId;
            
            // Remove from database
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            // Invalidate product cache
            await _cacheInvalidation.InvalidateProductCacheAsync((int)product.Id);
            
            // Invalidate category products cache
            await _cacheInvalidation.InvalidateProductsByCategoryAsync((int)categoryId);
            
            // Invalidate all products cache
            await _cacheInvalidation.InvalidateAllProductsCacheAsync();
            
            return true;
        }
    }
}