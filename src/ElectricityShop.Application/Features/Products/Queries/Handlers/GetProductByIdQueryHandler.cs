using System;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Common.Interfaces; // For IApplicationDbContext or repositories
using MediatR;
using Microsoft.Extensions.Logging; // Optional: for logging
using System.Collections.Generic; // For dummy data list

namespace ElectricityShop.Application.Features.Products.Queries.Handlers
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly ILogger<GetProductByIdQueryHandler> _logger;
        // private readonly IRepository<Product> _productRepository; // Or IApplicationDbContext

        // Dummy product list for simulation
        private static readonly List<ProductDto> _dummyProducts = new List<ProductDto>
        {
            new ProductDto { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Name = "Laptop Pro", Description = "High-end laptop", Price = 1200.00m, StockQuantity = 50, CategoryId = Guid.NewGuid(), CategoryName = "Electronics", IsActive = true },
            new ProductDto { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Name = "Wireless Mouse", Description = "Ergonomic wireless mouse", Price = 25.00m, StockQuantity = 200, CategoryId = Guid.NewGuid(), CategoryName = "Accessories", IsActive = true },
            new ProductDto { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Name = "Mechanical Keyboard", Description = "RGB Mechanical Keyboard", Price = 75.00m, StockQuantity = 100, CategoryId = Guid.NewGuid(), CategoryName = "Accessories", IsActive = false } // Inactive product
        };

        public GetProductByIdQueryHandler(ILogger<GetProductByIdQueryHandler> logger /*, IRepository<Product> productRepository */)
        {
            _logger = logger;
            // _productRepository = productRepository;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            _logger?.LogInformation("Attempting to fetch product by ID: {ProductId}", request.ProductId);

            // Simulate business logic:
            // 1. Query the database for the product.
            //    var product = await _productRepository.GetByIdAsync(request.ProductId);
            //    
            //    if (product == null || !product.IsActive) // Optionally filter by IsActive here or let client decide
            //    {
            //        _logger?.LogWarning("Product not found or not active. ProductId: {ProductId}", request.ProductId);
            //        return null;
            //    }
            //
            // 2. Map the Product entity to ProductDto.
            //    return new ProductDto
            //    {
            //        Id = product.Id,
            //        Name = product.Name,
            //        Description = product.Description,
            //        Price = product.Price,
            //        StockQuantity = product.StockQuantity,
            //        CategoryId = product.CategoryId,
            //        CategoryName = product.Category?.Name, // Assuming Category navigation property
            //        IsActive = product.IsActive
            //    };

            // Simulate async work
            await Task.Delay(50, cancellationToken);

            // Dummy data simulation:
            var product = _dummyProducts.FirstOrDefault(p => p.Id == request.ProductId);

            if (product != null) // For this simulation, we return even if IsActive is false, controller can decide
            {
                _logger?.LogInformation("Simulated product found for ProductId: {ProductId}", request.ProductId);
                return product;
            }

            _logger?.LogWarning("Simulated product not found for ProductId: {ProductId}", request.ProductId);
            return null; // Product not found
        }
    }
}
