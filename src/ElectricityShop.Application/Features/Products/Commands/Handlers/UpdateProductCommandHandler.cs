using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Features.Products.Commands.Dtos; // For ProductImageUpdateDto, ProductAttributeUpdateDto
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.Application.Features.Products.Commands.Handlers
{
    /// <summary>
    /// Handler for updating a product with cache invalidation
    /// </summary>
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly ILogger<UpdateProductCommandHandler> _logger;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;

        /// <summary>
        /// Initializes a new instance of the UpdateProductCommandHandler
        /// </summary>
        /// <param name="dbContext">Application DB context</param>
        /// <param name="cacheInvalidation">Cache invalidation service</param>
        public UpdateProductCommandHandler(
            ILogger<UpdateProductCommandHandler> logger,
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<bool> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update product with ID: {ProductId}", command.Id);

            var product = await _productRepository.GetByIdAsync(command.Id);
            if (product == null)
            {
                _logger.LogWarning("Product not found for update. ProductId: {ProductId}", command.Id);
                return false; // Product not found
            }

            // Validate Category (if changed)
            if (command.CategoryId != product.CategoryId)
            {
                if (command.CategoryId != Guid.Empty)
                {
                    var category = await _categoryRepository.GetByIdAsync(command.CategoryId);
                    if (category == null)
                    {
                        _logger.LogWarning("New CategoryId: {CategoryId} not found.", command.CategoryId);
                        return false; // New category not found
                    }
                    product.CategoryId = command.CategoryId;
                }
                else
                {
                    product.CategoryId = null; // Set to no category
                }
            }

            // Update basic properties
            product.Name = command.Name;
            product.Description = command.Description;
            product.Price = command.Price;
            product.StockQuantity = command.StockQuantity;
            product.IsActive = command.IsActive;
            // product.UpdatedAt = DateTime.UtcNow; // Handled by BaseEntity or interceptor typically

            // Update Images (Simplified: Clear and Add)
            // Ensure Images collection is not null before clearing, though it should be initialized by EF Core
            product.Images ??= new List<ProductImage>();
            product.Images.Clear(); // Clear existing images
            if (command.Images != null)
            {
                foreach (var imageDto in command.Images)
                {
                    product.Images.Add(new ProductImage 
                    { 
                        ImageUrl = imageDto.ImageUrl, 
                        IsMain = imageDto.IsMain 
                        // ProductId will be set by EF Core
                    });
                }
            }

            // Update Attributes (Simplified: Clear and Add)
            // Ensure Attributes collection is not null
            product.Attributes ??= new List<ProductAttribute>();
            product.Attributes.Clear(); // Clear existing attributes
            if (command.Attributes != null)
            {
                foreach (var attributeDto in command.Attributes)
                {
                    product.Attributes.Add(new ProductAttribute 
                    { 
                        Name = attributeDto.Name, 
                        Value = attributeDto.Value 
                        // ProductId will be set by EF Core
                    });
                }
            }

            // Persist Changes
            await _productRepository.UpdateAsync(product);
            
            _logger.LogInformation("Product updated successfully. ProductId: {ProductId}", command.Id);
            return true;
        }
    }
}
