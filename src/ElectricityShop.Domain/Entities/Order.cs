using System;
using System.Collections.Generic;

namespace ElectricityShop.Domain.Entities
{
    /// <summary>
    /// Represents an order
    /// </summary>
using ElectricityShop.Domain.Enums; // Required for OrderStatus and PaymentMethod

    public class Order : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user ID who placed the order
        /// </summary>
        public Guid UserId { get; set; } // Renamed from CustomerId for consistency
        public virtual User User { get; set; } // CS1061 fix: Added User navigation property
        
        /// <summary>
        /// Gets or sets the unique order number
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the order status
        /// </summary>
        public OrderStatus Status { get; set; } // Changed from string to Domain.Enums.OrderStatus
        
        /// <summary>
        /// Gets or sets the order date
        /// </summary>
        public DateTime OrderDate { get; set; }
        
        /// <summary>
        /// Gets or sets the total amount of the order
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the payment method used for the order
        /// </summary>
        public PaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// Gets or sets the shipping method selected for the order
        /// </summary>
        public string ShippingMethod { get; set; }

        /// <summary>
        /// Gets or sets any notes associated with the order
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the shipping address for the order
        /// </summary>
        public required Address ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the billing address for the order
        /// </summary>
        public required Address BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the subtotal of the order (sum of item prices before tax and shipping)
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Gets or sets the tax amount for the order
        /// </summary>
        public decimal Tax { get; set; }

        /// <summary>
        /// Gets or sets the shipping cost for the order
        /// </summary>
        public decimal ShippingCost { get; set; }
        
        /// <summary>
        /// Gets or sets the collection of items included in the order
        /// </summary>
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}