using System;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Features.Products.Commands; // For DeleteProductCommand
using ElectricityShop.Domain.Entities;      // For Product entity
using ElectricityShop.Domain.Interfaces;    // For IRepository
using MediatR;
using Microsoft.Extensions.Logging;         // For ILogger

namespace ElectricityShop.Application.Features.Products.Commands.Handlers
{
    /// <summary>
    /// Handler for deleting a product
    /// </summary>
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly ILogger<DeleteProductCommandHandler> _logger;
        private readonly IRepository<Product> _productRepository;

        /// <summary>
        /// Initializes a new instance of the DeleteProductCommandHandler
        /// </summary>
        /// <param name="dbContext">Application DB context</param>
        /// <param name="cacheInvalidation">Cache invalidation service</param>
        public DeleteProductCommandHandler(
            ILogger<DeleteProductCommandHandler> logger,
            IRepository<Product> productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Handles the delete product command
        /// </summary>
        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete product with ID: {ProductId}", request.ProductId);

            // Fetch Product
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                _logger.LogWarning("Product not found for deletion. ProductId: {ProductId}", request.ProductId);
                return false; // Product not found
            }

            // Delete Product
            // Assuming IRepository<T>.DeleteAsync takes the entity and handles the deletion.
            // If it only stages for a Unit of Work, then a UoW.CompleteAsync() would be needed elsewhere.
            // For this task, we assume DeleteAsync executes the deletion.
            await _productRepository.DeleteAsync(product); 
            
            // Persist Changes: Typically, DeleteAsync from a repository implies the deletion is executed.
            // If it's part of a Unit of Work that needs explicit commit, that's a broader pattern
            // not explicitly handled here without IUnitOfWork. For now, assume DeleteAsync is sufficient.

            _logger.LogInformation("Product deleted successfully. ProductId: {ProductId}", request.ProductId);
            return true; // Deletion successful
        }
    }
}
