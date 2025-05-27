using System;

namespace ElectricityShop.Domain.Entities
{
    /// <summary>
    /// Represents an order item
    /// </summary>
    public class OrderItem : BaseEntity
    {
        /// <summary>
        /// Gets or sets the order ID
        /// </summary>
        public Guid OrderId { get; set; }
        
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
        /// Gets the subtotal (quantity * unit price)
        /// </summary>
        public decimal Subtotal => Quantity * UnitPrice;
        
        /// <summary>
        /// Gets or sets the order
        /// </summary>
        public virtual Order Order { get; set; }
        
        /// <summary>
        /// Gets or sets the product associated with this order item
        /// </summary>
        public virtual Product Product { get; set; } // CS1061 fix: Added Product navigation property
    }
}