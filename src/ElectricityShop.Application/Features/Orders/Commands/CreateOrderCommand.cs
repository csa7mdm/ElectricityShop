using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Enums;
using ElectricityShop.Domain.Interfaces;
using MediatR;

namespace ElectricityShop.Application.Features.Orders.Commands
{
    public class CreateOrderCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();

        public class OrderItemDto
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }

        public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
        {
            private readonly IUnitOfWork _unitOfWork;

            public CreateOrderCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
            {
                // Generate a unique order number
                var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";

                // Create order
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = orderNumber,
                    UserId = request.UserId,
                    Status = OrderStatus.Pending,
                    PaymentMethod = request.PaymentMethod,
                    ShippingAddress = request.ShippingAddress,
                    BillingAddress = request.BillingAddress,
                    IsPaid = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Orders.AddAsync(order);
                
                decimal totalAmount = 0;

                // Add order items
                foreach (var item in request.Items)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    
                    if (product == null)
                    {
                        throw new Exception($"Product with id {item.ProductId} not found.");
                    }

                    if (product.StockQuantity < item.Quantity)
                    {
                        throw new Exception($"Not enough stock for product {product.Name}. Available: {product.StockQuantity}");
                    }

                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.OrderItems.AddAsync(orderItem);

                    // Update product stock
                    product.StockQuantity -= item.Quantity;
                    await _unitOfWork.Products.UpdateAsync(product);

                    // Calculate total
                    totalAmount += (product.Price * item.Quantity);
                }

                order.TotalAmount = totalAmount;
                await _unitOfWork.Orders.UpdateAsync(order);

                // Clear user's cart if exists
                var cart = (await _unitOfWork.Carts.ListAsync(c => c.UserId == request.UserId)).FirstOrDefault();
                if (cart != null)
                {
                    var cartItems = await _unitOfWork.CartItems.ListAsync(ci => ci.CartId == cart.Id);
                    foreach (var cartItem in cartItems)
                    {
                        await _unitOfWork.CartItems.DeleteAsync(cartItem);
                    }
                }

                await _unitOfWork.CompleteAsync();

                return order.Id;
            }
        }
    }
}