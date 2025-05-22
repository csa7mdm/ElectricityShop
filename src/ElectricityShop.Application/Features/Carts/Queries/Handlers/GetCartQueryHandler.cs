using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Features.Carts.Queries; // For CartDto, CartItemDto
using ElectricityShop.Domain.Entities;      // For Cart, CartItem, Product, ProductImage
using ElectricityShop.Domain.Interfaces;    // For IRepository
using MediatR;
using Microsoft.Extensions.Logging;         // For ILogger

namespace ElectricityShop.Application.Features.Carts.Queries.Handlers
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto?> // Return nullable CartDto
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Product> _productRepository; // Added as per refined instructions
        private readonly ILogger<GetCartQueryHandler> _logger;

        public GetCartQueryHandler(
            IRepository<Cart> cartRepository, 
            IRepository<Product> productRepository, // Added
            ILogger<GetCartQueryHandler> logger)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository; // Added
            _logger = logger;
        }

        public async Task<CartDto?> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to fetch cart for UserId: {UserId}", request.UserId);

            // Crucial: Repository implementation for FirstOrDefaultAsync (or a specific GetByUserIdAsync)
            // should handle .Include() for Items, Items.Product, and Product.Images.
            var cart = await _cartRepository.FirstOrDefaultAsync(c => c.UserId == request.UserId);

            if (cart == null)
            {
                _logger.LogWarning("No cart found for UserId: {UserId}", request.UserId);
                return null;
            }

            _logger.LogInformation("Cart found for UserId: {UserId}. Mapping to CartDto.", request.UserId);

            var cartDto = new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Items = new List<CartItemDto>()
            };

            foreach (var item in cart.Items)
            {
                Product? productDetails = item.Product;

                // Fallback: If product details were not loaded with the cart, fetch them.
                // This indicates an N+1 problem if it happens for many items.
                // The primary query for the cart should ideally include all necessary data.
                if (productDetails == null)
                {
                    _logger.LogWarning("Product details for ProductId {ProductId} in CartId {CartId} were not loaded. Fetching separately.", item.ProductId, cart.Id);
                    productDetails = await _productRepository.GetByIdAsync(item.ProductId);
                    if (productDetails == null)
                    {
                         _logger.LogError("Failed to fetch product details for ProductId {ProductId}. Skipping item.", item.ProductId);
                        continue; // Skip this item if product details can't be found
                    }
                }
                
                // Ensure UnitPrice in CartItem entity is used, as this is the price at the time of adding to cart.
                cartDto.Items.Add(new CartItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = productDetails.Name, 
                    UnitPrice = item.UnitPrice, // This should come from CartItem.UnitPrice
                    Quantity = item.Quantity,
                    // TotalPrice is a calculated property in CartItemDto (UnitPrice * Quantity)
                    ProductImageUrl = productDetails.Images?.FirstOrDefault(img => img.IsMain)?.ImageUrl
                });
            }
            
            // GrandTotal and TotalItems are calculated properties in CartDto.
            // They will be computed automatically based on the populated Items list.

            _logger.LogInformation("Successfully mapped cart to CartDto for UserId: {UserId}. Item count: {ItemCount}", request.UserId, cartDto.Items.Count);
            return cartDto;
        }
    }
}
