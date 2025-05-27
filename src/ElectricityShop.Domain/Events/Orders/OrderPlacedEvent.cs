using System;
using System.Collections.Generic;

namespace ElectricityShop.Domain.Events.Orders
{
    /// <summary>
    /// Event that is triggered when an order is placed
    /// </summary>
    public class OrderPlacedEvent : DomainEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderPlacedEvent"/> class
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <param name="customerId">The customer ID</param>
        /// <param name="orderItems">The order items</param>
        /// <param name="totalAmount">The total order amount</param>
        public OrderPlacedEvent(
            Guid orderId,
            Guid customerId,
            IEnumerable<OrderItemDto> orderItems,
            decimal totalAmount)
        {
            OrderId = orderId;
            CustomerId = customerId;
            OrderItems = orderItems;
            TotalAmount = totalAmount;
            OrderDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the order ID
        /// </summary>
        public Guid OrderId { get; }
        
        /// <summary>
        /// Gets the customer ID
        /// </summary>
        public Guid CustomerId { get; }
        
        /// <summary>
        /// Gets the order items
        /// </summary>
        public IEnumerable<OrderItemDto> OrderItems { get; }
        
        /// <summary>
        /// Gets the total order amount
        /// </summary>
        public decimal TotalAmount { get; }
        
        /// <summary>
        /// Gets the order date
        /// </summary>
        public DateTime OrderDate { get; }
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