using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Features.Carts.Commands; // For RemoveCartItemCommand
using ElectricityShop.Domain.Entities;      // For Cart, CartItem
using ElectricityShop.Domain.Interfaces;    // For IRepository
using MediatR;
using Microsoft.Extensions.Logging;         // For ILogger
using System.Collections.Generic;           // For List<CartItem>

namespace ElectricityShop.Application.Features.Carts.Commands.Handlers
{
    public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, bool>
    {
        private readonly ILogger<RemoveCartItemCommandHandler> _logger;
        private readonly IRepository<Cart> _cartRepository;

        public RemoveCartItemCommandHandler(
            ILogger<RemoveCartItemCommandHandler> logger,
            IRepository<Cart> cartRepository)
        {
            _logger = logger;
            _cartRepository = cartRepository;
        }

        public async Task<bool> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to remove cart item. UserId: {UserId}, ProductId: {ProductId}",
                request.UserId, request.ProductId);

            // Fetch Cart (ensure Items are loaded by repository configuration)
            var cart = await _cartRepository.FirstOrDefaultAsync(c => c.UserId == request.UserId);
            if (cart == null)
            {
                _logger.LogWarning("Cart not found for UserId: {UserId}", request.UserId);
                return false; // Cart not found
            }
            cart.Items ??= new List<CartItem>(); // Ensure Items collection is initialized

            // Find CartItem using request.ProductId
            var cartItemToRemove = cart.Items.FirstOrDefault(ci => ci.ProductId == request.ProductId);
            if (cartItemToRemove == null)
            {
                _logger.LogWarning("CartItem with ProductId: {ProductId} not found in CartId: {CartId} for UserId: {UserId}",
                    request.ProductId, cart.Id, request.UserId);
                return false; // Item not in cart
            }

            // Remove Item
            bool removed = cart.Items.Remove(cartItemToRemove);
            if (!removed)
            {
                // This case should ideally not be hit if FirstOrDefault found an item,
                // but it's a safeguard or indicates a potential issue with collection management.
                _logger.LogError("Failed to remove ProductId: {ProductId} from CartId: {CartId}'s Items collection, though it was found.",
                    request.ProductId, cart.Id);
                return false; 
            }
            
            _logger.LogInformation("ProductId: {ProductId} removed from CartId: {CartId}'s Items collection.", request.ProductId, cart.Id);

            // Persist Changes
            await _cartRepository.UpdateAsync(cart);

            _logger.LogInformation("Cart item removal processed successfully for UserId: {UserId}, ProductId: {ProductId}, CartId: {CartId}",
                request.UserId, request.ProductId, cart.Id);
            return true;
        }
    }
}
