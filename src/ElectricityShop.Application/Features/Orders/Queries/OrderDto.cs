using System;
using System.Collections.Generic;

namespace ElectricityShop.Application.Features.Orders.Queries
{
    /// <summary>
    /// Data transfer object for order data
    /// </summary>
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        
        /// <summary>
        /// Current status of the order
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Total amount of the order
        /// </summary>
        public decimal TotalAmount { get; set; }
        
        /// <summary>
        /// User who placed the order
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Items in the order
        /// </summary>
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        
        /// <summary>
        /// Shipping address for the order
        /// </summary>
        public AddressDto ShippingAddress { get; set; }
        // Add other relevant properties like BillingAddress, PaymentInfoSummary etc. if needed
    }
}
