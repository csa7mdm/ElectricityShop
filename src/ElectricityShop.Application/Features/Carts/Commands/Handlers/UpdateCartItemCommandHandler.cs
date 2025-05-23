using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Features.Carts.Commands; // For UpdateCartItemCommand
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic; // For List<CartItem>

namespace ElectricityShop.Application.Features.Carts.Commands.Handlers
{
    public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, bool>
    {
        private readonly ILogger<UpdateCartItemCommandHandler> _logger;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Cart> _cartRepository;

        public UpdateCartItemCommandHandler(
            ILogger<UpdateCartItemCommandHandler> logger,
            IRepository<Product> productRepository,
            IRepository<Cart> cartRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _cartRepository = cartRepository;
        }

        public async Task<bool> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update cart item. UserId: {UserId}, ProductId: {ProductId}, NewQuantity: {NewQuantity}",
                request.UserId, request.ProductId, request.NewQuantity);

            // Fetch Cart (ensure Items are loaded by repository configuration)
            var cart = await _cartRepository.FirstOrDefaultAsync(c => c.UserId == request.UserId);
            if (cart == null)
            {
                _logger.LogWarning("Cart not found for UserId: {UserId}", request.UserId);
                return false; // Cart not found
            }
            cart.Items ??= new List<CartItem>(); // Ensure Items collection is initialized

            // Find CartItem using request.ProductId (which identifies the Product in the cart)
            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == request.ProductId);
            if (cartItem == null)
            {
                _logger.LogWarning("CartItem with ProductId: {ProductId} not found in CartId: {CartId} for UserId: {UserId}",
                    request.ProductId, cart.Id, request.UserId);
                return false; // Item not in cart
            }

            // If NewQuantity is 0, remove the item
            if (request.NewQuantity == 0)
            {
                _logger.LogInformation("NewQuantity is 0. Removing ProductId: {ProductId} from CartId: {CartId}", request.ProductId, cart.Id);
                cart.Items.Remove(cartItem);
            }
            else // NewQuantity > 0, update quantity
            {
                // Fetch Product for stock validation
                var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                if (product == null)
                {
                    _logger.LogError("Product details not found for ProductId: {ProductId} in CartItem {CartItemId}. This indicates a data integrity issue.",
                        cartItem.ProductId, cartItem.Id);
                    return false; // Should not happen if item is in cart, but safety check
                }

                // Check Stock if quantity is increasing
                if (request.NewQuantity > cartItem.Quantity)
                {
                    int additionalQuantityNeeded = request.NewQuantity - cartItem.Quantity;
                    if (product.StockQuantity < additionalQuantityNeeded)
                    {
                        _logger.LogWarning("Insufficient stock for ProductId: {ProductId} during update. Additional needed: {AdditionalQuantityNeeded}, Available: {AvailableStock}",
                            request.ProductId, additionalQuantityNeeded, product.StockQuantity);
                        return false; // Insufficient stock for the increase
                    }
                }
                cartItem.Quantity = request.NewQuantity;
            }

            // Persist Changes
            await _cartRepository.UpdateAsync(cart);

            _logger.LogInformation("Cart item update/removal processed successfully for UserId: {UserId}, ProductId: {ProductId}, CartId: {CartId}",
                request.UserId, request.ProductId, cart.Id);
            return true;
        }
    }
}
