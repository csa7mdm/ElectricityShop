using System;
using System.Collections.Generic;

namespace ElectricityShop.Domain.Entities
{
    /// <summary>
    /// Represents an order
    /// </summary>
    public class Order : BaseEntity
    {
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
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}