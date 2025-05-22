using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Features.Carts.Commands; // For AddItemToCartCommand
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic; // For List<CartItem>

namespace ElectricityShop.Application.Features.Carts.Commands.Handlers
{
    public class AddItemToCartCommandHandler : IRequestHandler<AddItemToCartCommand, Unit>
    {
        private readonly ILogger<AddItemToCartCommandHandler> _logger;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<CartItem> _cartItemRepository; // Assuming this might be used

        public AddItemToCartCommandHandler(
            ILogger<AddItemToCartCommandHandler> logger,
            IRepository<Product> productRepository,
            IRepository<Cart> cartRepository,
            IRepository<CartItem> cartItemRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
        }

        public async Task<Unit> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to add item to cart. UserId: {UserId}, ProductId: {ProductId}, Quantity: {Quantity}", 
                request.UserId, request.ProductId, request.Quantity);

            // 1. Fetch Product
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                _logger.LogWarning("Product not found. ProductId: {ProductId}", request.ProductId);
                throw new ApplicationException($"Product with ID {request.ProductId} not found."); // Consider specific NotFoundException
            }
            if (!product.IsActive)
            {
                _logger.LogWarning("Product is not active. ProductId: {ProductId}", request.ProductId);
                throw new ApplicationException($"Product {product.Name} is currently unavailable.");
            }

            // 2. Check Stock
            // This check is for the requested quantity. If item exists, stock for total quantity is implicitly handled by domain/db.
            if (product.StockQuantity < request.Quantity)
            {
                _logger.LogWarning("Insufficient stock for ProductId: {ProductId}. Requested: {RequestedQuantity}, Available: {AvailableStock}", 
                    request.ProductId, request.Quantity, product.StockQuantity);
                throw new ApplicationException($"Insufficient stock for product {product.Name}. Available: {product.StockQuantity}, Requested: {request.Quantity}.");
            }

            // 3. Fetch or Create Cart
            // Assuming FirstOrDefaultAsync on IRepository<Cart> loads cart.Items or it's handled by the ORM.
            // If Items are not loaded, cart.Items.FirstOrDefault below will not work as expected for existing items.
            var cart = await _cartRepository.FirstOrDefaultAsync(c => c.UserId == request.UserId);
            bool isNewCart = false;

            if (cart == null)
            {
                _logger.LogInformation("No existing cart found for UserId: {UserId}. Creating a new cart.", request.UserId);
                cart = new Cart { UserId = request.UserId, Items = new List<CartItem>() };
                await _cartRepository.AddAsync(cart); 
                isNewCart = true;
                // Assuming AddAsync for a new cart also makes it ready for UpdateAsync if items are managed through it,
                // or if items are managed separately, the cart itself is now persisted/tracked.
            }
            else
            {
                _logger.LogInformation("Existing cart found for UserId: {UserId}. CartId: {CartId}", request.UserId, cart.Id);
                // Ensure Items collection is initialized if it's null (can happen if not loaded correctly by repo)
                cart.Items ??= new List<CartItem>();
            }

            // 4. Find or Create CartItem
            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == request.ProductId);

            if (cartItem != null)
            {
                _logger.LogInformation("Product {ProductId} already in cart {CartId}. Updating quantity.", request.ProductId, cart.Id);
                // Optional: Re-check stock for the new total quantity
                if (product.StockQuantity < (cartItem.Quantity + request.Quantity)) {
                     _logger.LogWarning("Insufficient stock for ProductId: {ProductId} when updating quantity. Requested total: {RequestedTotal}, Available: {AvailableStock}", 
                        request.ProductId, cartItem.Quantity + request.Quantity, product.StockQuantity);
                    throw new ApplicationException($"Insufficient stock for product {product.Name} when updating quantity.");
                }
                cartItem.Quantity += request.Quantity;
                // If CartItem is an independent entity that needs explicit update:
                // await _cartItemRepository.UpdateAsync(cartItem); 
            }
            else
            {
                _logger.LogInformation("Product {ProductId} not in cart {CartId}. Adding new cart item.", request.ProductId, cart.Id);
                cartItem = new CartItem
                {
                    // CartId will be set by EF Core if cart.Id is already generated (for new carts after AddAsync)
                    // or it's an existing cart. If cart is new and Id isn't set yet, this needs care.
                    // For now, assuming cart.Id is valid here.
                    CartId = cart.Id, 
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = product.Price // Store current price at the time of adding
                };
                cart.Items.Add(cartItem);
                // If CartItem is an independent entity that needs explicit add:
                // await _cartItemRepository.AddAsync(cartItem);
            }
            
            // 5. Persist Changes
            // Assuming UpdateAsync on cart repository saves the cart and its item collection changes.
            // If the cart was new, AddAsync might have already saved it, or an Update is still needed
            // if AddAsync only stages it in a UoW. Given the prompt, we'll call UpdateAsync.
            // If IUnitOfWork.CompleteAsync() was available, it would be called here.
            if (!isNewCart) // Only update if it's an existing cart that was modified
            {
                 await _cartRepository.UpdateAsync(cart);
            }
            // If it was a new cart, AddAsync might have been enough, or an UpdateAsync might be needed
            // if AddAsync only adds to context and doesn't save. This depends on IRepository implementation.
            // To be safe, if items were added to a new cart, an Update might be needed.
            // However, if AddAsync saves, this UpdateAsync on a new cart could be redundant or even problematic.
            // Let's assume for now that AddAsync on a new cart is sufficient if items are managed via collection,
            // and if items were added via _cartItemRepository.AddAsync, that's handled.
            // The most robust way is with a Unit of Work.
            // For simplicity stated in prompt: "assume _cartRepository.AddAsync(newCart) and 
            // _cartRepository.UpdateAsync(existingCart) are sufficient".
            // If it was a new cart, AddAsync for the cart entity was called.
            // Now, regardless of whether the cart was new or existing, if its Items collection was modified,
            // we need to persist these changes. Assuming IRepository<Cart>.UpdateAsync handles this
            // by saving the aggregate root (Cart) and its related entities (CartItems).
            await _cartRepository.UpdateAsync(cart);

            _logger.LogInformation("Item successfully added/updated in cart. UserId: {UserId}, ProductId: {ProductId}, CartId: {CartId}", 
                request.UserId, request.ProductId, cart.Id);

            return Unit.Value;
        }
    }
}
