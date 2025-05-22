using System;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Features.Carts.Commands; // For ClearCartCommand
using ElectricityShop.Domain.Entities;      // For Cart
using ElectricityShop.Domain.Interfaces;    // For IRepository
using MediatR;
using Microsoft.Extensions.Logging;         // For ILogger
using System.Collections.Generic;           // For List<CartItem>

namespace ElectricityShop.Application.Features.Carts.Commands.Handlers
{
    public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Unit>
    {
        private readonly ILogger<ClearCartCommandHandler> _logger;
        private readonly IRepository<Cart> _cartRepository;

        public ClearCartCommandHandler(
            ILogger<ClearCartCommandHandler> logger,
            IRepository<Cart> cartRepository)
        {
            _logger = logger;
            _cartRepository = cartRepository;
        }

        public async Task<Unit> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to clear cart for UserId: {UserId}", request.UserId);

            // Fetch Cart
            var cart = await _cartRepository.FirstOrDefaultAsync(c => c.UserId == request.UserId);

            if (cart == null)
            {
                _logger.LogInformation("No cart found for UserId: {UserId}. Operation considered successful as cart is already clear.", request.UserId);
                return Unit.Value; // Idempotent: if no cart, it's "clear"
            }

            // Clear Items if cart exists
            // Ensure Items collection is not null before clearing, though it should be initialized by EF Core
            cart.Items ??= new List<CartItem>();
            if (cart.Items.Any()) 
            {
                _logger.LogInformation("Cart found for UserId: {UserId} with {ItemCount} items. Clearing items.", request.UserId, cart.Items.Count);
                cart.Items.Clear();

                // Persist Changes
                await _cartRepository.UpdateAsync(cart);
                _logger.LogInformation("Cart items cleared and changes persisted for UserId: {UserId}, CartId: {CartId}", request.UserId, cart.Id);
            }
            else
            {
                _logger.LogInformation("Cart found for UserId: {UserId} but it already has no items. No changes needed.", request.UserId);
            }
            
            return Unit.Value;
        }
    }
}
