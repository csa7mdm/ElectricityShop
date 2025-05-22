using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Common.Interfaces; // For IApplicationDbContext or repositories
using ElectricityShop.Application.Features.Products.Queries; // For ProductDto (for dummy data)
using MediatR;
using Microsoft.Extensions.Logging; // Optional: for logging
using System.Collections.Generic; // For dummy data list

namespace ElectricityShop.Application.Features.Products.Commands.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly ILogger<UpdateProductCommandHandler> _logger;
        // private readonly IRepository<Product> _productRepository; // Or IApplicationDbContext

        // Re-use dummy product list from GetProductByIdQueryHandler for simulation consistency
        private static readonly List<ProductDto> _dummyProducts = new List<ProductDto>
        {
            new ProductDto { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Name = "Laptop Pro", Description = "High-end laptop", Price = 1200.00m, StockQuantity = 50, CategoryId = Guid.NewGuid(), CategoryName = "Electronics", IsActive = true },
            new ProductDto { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Name = "Wireless Mouse", Description = "Ergonomic wireless mouse", Price = 25.00m, StockQuantity = 200, CategoryId = Guid.NewGuid(), CategoryName = "Accessories", IsActive = true },
            new ProductDto { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Name = "Mechanical Keyboard", Description = "RGB Mechanical Keyboard", Price = 75.00m, StockQuantity = 100, CategoryId = Guid.NewGuid(), CategoryName = "Accessories", IsActive = false }
        };

        public UpdateProductCommandHandler(ILogger<UpdateProductCommandHandler> logger /*, IRepository<Product> productRepository */)
        {
            _logger = logger;
            // _productRepository = productRepository;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            _logger?.LogInformation("Attempting to update product with ID: {ProductId}", request.Id);

            // Simulate business logic:
            // 1. Find the product in the database.
            //    var product = await _productRepository.GetByIdAsync(request.Id);
            //    if (product == null)
            //    {
            //        _logger?.LogWarning("Product not found for update. ProductId: {ProductId}", request.Id);
            //        return false; // Product not found
            //    }

            // 2. Update product properties.
            //    product.Name = request.Name;
            //    product.Description = request.Description;
            //    product.Price = request.Price;
            //    product.StockQuantity = request.StockQuantity;
            //    product.CategoryId = request.CategoryId;
            //    product.IsActive = request.IsActive;
            //    // product.UpdatedAt = DateTime.UtcNow; // If you have an UpdatedAt property

            // 3. Save changes.
            //    await _productRepository.UpdateAsync(product);
            //    _logger?.LogInformation("Product updated successfully. ProductId: {ProductId}", request.Id);
            //    return true;

            // Simulate async work
            await Task.Delay(50, cancellationToken);

            // Dummy data simulation:
            var productToUpdate = _dummyProducts.FirstOrDefault(p => p.Id == request.Id);

            if (productToUpdate == null)
            {
                _logger?.LogWarning("Simulated product not found for update. ProductId: {ProductId}", request.Id);
                return false; // Product not found
            }

            // Apply updates to the dummy product
            productToUpdate.Name = request.Name;
            productToUpdate.Description = request.Description;
            productToUpdate.Price = request.Price;
            productToUpdate.StockQuantity = request.StockQuantity;
            productToUpdate.CategoryId = request.CategoryId;
            // For CategoryName, in a real scenario, you might fetch it based on CategoryId or it might not be directly updatable here.
            // For simulation, we'll just update it if CategoryId changes, assuming a lookup.
            productToUpdate.CategoryName = $"Category for {request.CategoryId.ToString().Substring(0, 4)}"; 
            productToUpdate.IsActive = request.IsActive;
            
            _logger?.LogInformation("Simulated product updated successfully. ProductId: {ProductId}", request.Id);
            return true; // Update successful
        }
    }
}
