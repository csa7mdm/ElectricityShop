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
        
        /// <summary>
        /// Billing address for the order
        /// </summary>
        public AddressDto BillingAddress { get; set; }
        
        /// <summary>
        /// Payment method used
        /// </summary>
        public string PaymentMethod { get; set; }
        
        /// <summary>
        /// Payment status
        /// </summary>
        public string PaymentStatus { get; set; }
        
        /// <summary>
        /// Shipping method
        /// </summary>
        public string ShippingMethod { get; set; }
        
        /// <summary>
        /// Shipping cost
        /// </summary>
        public decimal ShippingCost { get; set; }
        
        /// <summary>
        /// Tax amount
        /// </summary>
        public decimal TaxAmount { get; set; }
        
        /// <summary>
        /// Notes for the order
        /// </summary>
        public string Notes { get; set; }
    }
    
    /// <summary>
    /// Data transfer object for order item data
    /// </summary>
    public class OrderItemDto
    {
        /// <summary>
        /// Unique identifier for the order item
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Product ID
        /// </summary>
        public Guid ProductId { get; set; }
        
        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; }
        
        /// <summary>
        /// Product price at the time of order
        /// </summary>
        public decimal UnitPrice { get; set; }
        
        /// <summary>
        /// Quantity ordered
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// Subtotal (UnitPrice * Quantity)
        /// </summary>
        public decimal Subtotal { get; set; }
    }
    
    /// <summary>
    /// Data transfer object for address data
    /// </summary>
    public class AddressDto
    {
        /// <summary>
        /// First line of the address
        /// </summary>
        public string Line1 { get; set; }
        
        /// <summary>
        /// Second line of the address (optional)
        /// </summary>
        public string Line2 { get; set; }
        
        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }
        
        /// <summary>
        /// State or province
        /// </summary>
        public string State { get; set; }
        
        /// <summary>
        /// Postal or ZIP code
        /// </summary>
        public string PostalCode { get; set; }
        
        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }
    }
}
