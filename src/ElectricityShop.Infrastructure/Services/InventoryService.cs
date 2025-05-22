using System;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.Infrastructure.Services
{
    /// <summary>
    /// Service for managing inventory
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InventoryService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryService"/> class
        /// </summary>
        /// <param name="unitOfWork">The unit of work</param>
        /// <param name="logger">The logger</param>
        public InventoryService(
            IUnitOfWork unitOfWork,
            ILogger<InventoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Deducts inventory for a product
        /// </summary>
        /// <param name="productId">The product ID</param>
        /// <param name="quantity">The quantity to deduct</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task DeductInventoryAsync(
            Guid productId, 
            int quantity, 
            CancellationToken cancellationToken = default)
        {
            var productRepository = _unitOfWork.GetRepository<Product>();
            var product = await productRepository.GetByIdAsync(productId);
            
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found");
            }
            
            if (product.StockQuantity < quantity)
            {
                throw new InvalidOperationException($"Not enough inventory for product {product.Name}");
            }
            
            product.StockQuantity -= quantity;
            
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation(
                "Deducted {Quantity} units from inventory for product {ProductId}", 
                quantity, productId);
        }

        /// <summary>
        /// Returns inventory for a product
        /// </summary>
        /// <param name="productId">The product ID</param>
        /// <param name="quantity">The quantity to return</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task ReturnInventoryAsync(
            Guid productId, 
            int quantity, 
            CancellationToken cancellationToken = default)
        {
            var productRepository = _unitOfWork.GetRepository<Product>();
            var product = await productRepository.GetByIdAsync(productId);
            
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found");
            }
            
            product.StockQuantity += quantity;
            
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation(
                "Returned {Quantity} units to inventory for product {ProductId}", 
                quantity, productId);
        }

        /// <summary>
        /// Checks if a product has enough inventory
        /// </summary>
        /// <param name="productId">The product ID</param>
        /// <param name="quantity">The quantity to check</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>True if the product has enough inventory; otherwise, false</returns>
        public async Task<bool> HasEnoughInventoryAsync(
            Guid productId, 
            int quantity, 
            CancellationToken cancellationToken = default)
        {
            var productRepository = _unitOfWork.GetRepository<Product>();
            var product = await productRepository.GetByIdAsync(productId);
            
            if (product == null)
            {
                return false;
            }
            
            return product.StockQuantity >= quantity;
        }
    }
}