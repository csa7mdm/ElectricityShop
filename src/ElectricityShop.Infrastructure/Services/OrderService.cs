using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Common.Events;
using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Domain.Entities;
using ElectricityShop.Domain.Events.Orders;
using ElectricityShop.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.Infrastructure.Services
{
    /// <summary>
    /// Service for managing orders
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;
        private readonly ILogger<OrderService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderService"/> class
        /// </summary>
        /// <param name="unitOfWork">The unit of work</param>
        /// <param name="eventBus">The event bus</param>
        /// <param name="logger">The logger</param>
        public OrderService(
            IUnitOfWork unitOfWork,
            IEventBus eventBus,
            ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
            _logger = logger;
        }

        /// <summary>
        /// Gets an order by ID
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The order</returns>
        public async Task<OrderDto> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var orderRepository = _unitOfWork.GetRepository<Order>();
            var order = await orderRepository.GetByIdAsync(orderId);
            
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found");
            }

            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Status = order.Status,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };
        }

        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <param name="items">The order items</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The created order ID</returns>
        public async Task<Guid> CreateOrderAsync(
            Guid customerId, 
            IEnumerable<OrderItemDto> items,
            CancellationToken cancellationToken = default)
        {
            var orderRepository = _unitOfWork.GetRepository<Order>();
            
            // Calculate the total amount
            var totalAmount = items.Sum(item => item.Quantity * item.UnitPrice);
            
            // Create the order
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Status = "Pending",
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Items = items.Select(item => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };
            
            // Add the order to the repository
            await orderRepository.AddAsync(order);
            
            // Save changes
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Created order {OrderId} for customer {CustomerId}", order.Id, customerId);
            
            // Create and publish the OrderPlacedEvent
            var orderPlacedEvent = new OrderPlacedEvent(
                order.Id,
                order.CustomerId,
                order.Items.Select(item => new Domain.Events.Orders.OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }),
                order.TotalAmount);
            
            await _eventBus.PublishAsync(orderPlacedEvent, cancellationToken);
            
            _logger.LogInformation("Published OrderPlacedEvent for order {OrderId}", order.Id);
            
            return order.Id;
        }

        /// <summary>
        /// Updates an order status
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <param name="newStatus">The new status</param>
        /// <param name="updatedById">The ID of the user who updated the status</param>
        /// <param name="notes">Optional notes about the status change</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task UpdateOrderStatusAsync(
            Guid orderId, 
            string newStatus, 
            Guid? updatedById = null, 
            string notes = null, 
            CancellationToken cancellationToken = default)
        {
            var orderRepository = _unitOfWork.GetRepository<Order>();
            var order = await orderRepository.GetByIdAsync(orderId);
            
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found");
            }
            
            var previousStatus = order.Status;
            
            // Update the order status
            order.Status = newStatus;
            
            // Save changes
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation(
                "Updated order {OrderId} status from {PreviousStatus} to {NewStatus}", 
                orderId, previousStatus, newStatus);
            
            // Create and publish the OrderStatusChangedEvent
            var orderStatusChangedEvent = new OrderStatusChangedEvent(
                order.Id,
                order.CustomerId,
                previousStatus,
                newStatus,
                updatedById,
                notes);
            
            await _eventBus.PublishAsync(orderStatusChangedEvent, cancellationToken);
            
            _logger.LogInformation("Published OrderStatusChangedEvent for order {OrderId}", order.Id);
        }

        /// <summary>
        /// Cancels an order
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <param name="reason">The cancellation reason</param>
        /// <param name="cancelledById">The ID of the user who cancelled the order</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task CancelOrderAsync(
            Guid orderId, 
            string reason, 
            Guid? cancelledById = null, 
            CancellationToken cancellationToken = default)
        {
            var orderRepository = _unitOfWork.GetRepository<Order>();
            var order = await orderRepository.GetByIdAsync(orderId);
            
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found");
            }
            
            // Check if the order can be cancelled
            if (order.Status == "Shipped" || order.Status == "Delivered" || order.Status == "Cancelled")
            {
                throw new InvalidOperationException($"Cannot cancel order with status {order.Status}");
            }
            
            // Update the order status
            order.Status = "Cancelled";
            
            // Save changes
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Cancelled order {OrderId}", orderId);
            
            // Create and publish the OrderCancelledEvent
            var orderCancelledEvent = new OrderCancelledEvent(
                order.Id,
                order.CustomerId,
                reason,
                cancelledById);
            
            await _eventBus.PublishAsync(orderCancelledEvent, cancellationToken);
            
            _logger.LogInformation("Published OrderCancelledEvent for order {OrderId}", order.Id);
        }
    }
}