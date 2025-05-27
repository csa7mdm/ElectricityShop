using ElectricityShop.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Common.Interfaces; // For IApplicationDbContext or if IRepository is there
using ElectricityShop.Application.Features.Products.Queries; // For ProductDto, ProductImageDto, ProductAttributeDto
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.Application.Features.Products.Queries.Handlers
{
    /// <summary>
    /// Handler for retrieving a single product by ID
    /// </summary>
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly ILogger<GetProductByIdQueryHandler> _logger;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;

        /// <summary>
        /// Initializes a new instance of GetProductByIdQueryHandler
        /// </summary>
        /// <param name="dbContext">Application DB context</param>
        /// <param name="cacheService">Cache service for optimized retrieval</param>
        public GetProductByIdQueryHandler(
            ILogger<GetProductByIdQueryHandler> logger,
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Handles the query, retrieving from cache if available
        /// </summary>
        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            _logger?.LogInformation("Attempting to fetch product by ID: {ProductId} from database", request.ProductId);

            // The IRepository.GetByIdAsync is expected to handle includes or eager loading if necessary.
            // For this example, we assume product.Images and product.Attributes are populated.
            // Category might need a separate fetch if not included.
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            
            if (product == null)
            {
                _logger?.LogWarning("Product not found in database. ProductId: {ProductId}", request.ProductId);
                return null;
            }

            string categoryName = "N/A"; // Default category name
            if (product.CategoryId != Guid.Empty)
            {
                // Attempt to get category name.
                // If product.Category is null (not eagerly loaded with the product), fetch it.
                var category = product.Category ?? await _categoryRepository.GetByIdAsync(product.CategoryId);
                if (category != null)
                {
                    categoryName = category.Name;
                }
                else
                {
                     _logger?.LogWarning("Category not found for CategoryId: {CategoryId} associated with ProductId: {ProductId}", product.CategoryId, product.Id);
                }
            }
            else
            {
                 _logger?.LogInformation("Product {ProductId} has no CategoryId.", product.Id);
            }
            
            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                CategoryName = categoryName,
                IsActive = product.IsActive,
                Images = product.Images?.Select(img => new ProductImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    IsMain = img.IsMain
                }).ToList() ?? new List<ProductImageDto>(),
                Attributes = product.Attributes?.Select(attr => new ProductAttributeDto
                {
                    Id = attr.Id,
                    Name = attr.Name,
                    Value = attr.Value
                }).ToList() ?? new List<ProductAttributeDto>()
            };

            _logger?.LogInformation("Product found and mapped to DTO. ProductId: {ProductId}", request.ProductId);
            return productDto;
        }
    }
}
