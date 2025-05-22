using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for order services
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Gets an order by ID
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The order</returns>
        Task<OrderDto> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <param name="items">The order items</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The created order ID</returns>
        Task<Guid> CreateOrderAsync(
            Guid customerId, 
            IEnumerable<OrderItemDto> items,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an order status
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <param name="newStatus">The new status</param>
        /// <param name="updatedById">The ID of the user who updated the status</param>
        /// <param name="notes">Optional notes about the status change</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task UpdateOrderStatusAsync(
            Guid orderId, 
            string newStatus, 
            Guid? updatedById = null, 
            string notes = null, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels an order
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <param name="reason">The cancellation reason</param>
        /// <param name="cancelledById">The ID of the user who cancelled the order</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task CancelOrderAsync(
            Guid orderId, 
            string reason, 
            Guid? cancelledById = null, 
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Data transfer object for orders
    /// </summary>
    public class OrderDto
    {
        /// <summary>
        /// Gets or sets the order ID
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Gets or sets the customer ID
        /// </summary>
        public Guid CustomerId { get; set; }
        
        /// <summary>
        /// Gets or sets the order status
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Gets or sets the order date
        /// </summary>
        public DateTime OrderDate { get; set; }
        
        /// <summary>
        /// Gets or sets the total amount
        /// </summary>
        public decimal TotalAmount { get; set; }
        
        /// <summary>
        /// Gets or sets the order items
        /// </summary>
        public IEnumerable<OrderItemDto> Items { get; set; }
    }

    /// <summary>
    /// Data transfer object for order items
    /// </summary>
    public class OrderItemDto
    {
        /// <summary>
        /// Gets or sets the product ID
        /// </summary>
        public Guid ProductId { get; set; }
        
        /// <summary>
        /// Gets or sets the product name
        /// </summary>
        public string ProductName { get; set; }
        
        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// Gets or sets the unit price
        /// </summary>
        public decimal UnitPrice { get; set; }
        
        /// <summary>
        /// Gets or sets the subtotal
        /// </summary>
        public decimal Subtotal => Quantity * UnitPrice;
    }
}