using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Features.Products.Queries; // For ProductDto etc.
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.Application.Features.Products.Queries.Handlers
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {
        private readonly ILogger<GetProductsQueryHandler> _logger;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;

        public GetProductsQueryHandler(
            ILogger<GetProductsQueryHandler> logger,
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            _logger?.LogInformation("Attempting to fetch products from database with filters. SearchTerm: '{SearchTerm}', CategoryId: '{CategoryId}'", 
                request.SearchTerm, request.CategoryId);

            // Fetch all products - Inefficient for large datasets, filtering should ideally happen at the database level.
            // This is done because the IRepository<T> interface used (GetAllAsync) does not support IQueryable or Specifications directly.
            // A more performant solution would involve extending IRepository or using a specific query method.
            var allProducts = await _productRepository.GetAllAsync();
            IEnumerable<Product> filteredProducts = allProducts;

            // Apply SearchTerm filter (in-memory)
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                string searchTermLower = request.SearchTerm.ToLowerInvariant();
                filteredProducts = filteredProducts.Where(p => 
                    (p.Name != null && p.Name.ToLowerInvariant().Contains(searchTermLower)) || 
                    (p.Description != null && p.Description.ToLowerInvariant().Contains(searchTermLower)));
            }

            // Apply CategoryId filter (in-memory)
            if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
            {
                filteredProducts = filteredProducts.Where(p => p.CategoryId == request.CategoryId.Value);
            }
            
            var products = filteredProducts.ToList(); // Materialize the filtered list

            if (products == null || !products.Any())
            {
                _logger?.LogInformation("No products found in the database.");
                return new List<ProductDto>();
            }

            var categoriesList = await _categoryRepository.GetAllAsync();
            var categoriesDictionary = categoriesList?.ToDictionary(c => c.Id) ?? new Dictionary<Guid, Category>();

            var productDtos = new List<ProductDto>();

            foreach (var product in products)
            {
                string categoryName = "N/A"; // Default
                if (product.CategoryId != Guid.Empty && categoriesDictionary.TryGetValue(product.CategoryId, out var category))
                {
                    categoryName = category.Name;
                }
                else if (product.CategoryId != Guid.Empty)
                {
                    _logger?.LogWarning("Category not found in dictionary for CategoryId: {CategoryId} associated with ProductId: {ProductId}", product.CategoryId, product.Id);
                }

                productDtos.Add(new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description, 
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    CategoryId = product.CategoryId,
                    CategoryName = categoryName,
                    IsActive = product.IsActive,
                    Images = new List<ProductImageDto>(), // Empty list for list view
                    Attributes = new List<ProductAttributeDto>() // Empty list for list view
                });
            }

            _logger?.LogInformation("Successfully fetched and mapped {ProductCount} products.", productDtos.Count);
            return productDtos;
        }
    }
}
