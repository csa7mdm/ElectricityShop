using MediatR;
using System;
using System.Collections.Generic;
using ElectricityShop.Application.Features.Orders.Models;

namespace ElectricityShop.Application.Features.Orders.Commands
{
    /// <summary>
    /// Command to create a new order
    /// </summary>
    public class CreateOrderCommand : IRequest<OrderCreationResult>
    {
        /// <summary>
        /// User ID placing the order
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
        /// Payment method to use
        /// </summary>
        public string PaymentMethod { get; set; }
        
        /// <summary>
        /// Shipping method to use
        /// </summary>
        public string ShippingMethod { get; set; }
        
        /// <summary>
        /// Notes for the order
        /// </summary>
        public string Notes { get; set; }
    }
    
    /// <summary>
    /// Data transfer object for order item input
    /// </summary>
    public class OrderItemDto
    {
        /// <summary>
        /// Product ID
        /// </summary>
        public Guid ProductId { get; set; }
        
        /// <summary>
        /// Quantity to order
        /// </summary>
        public int Quantity { get; set; }
    }
    
    /// <summary>
    /// Data transfer object for address input
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