using System;
using ElectricityShop.Application.Features.Orders.Queries; // For BillingAddressDto
using MediatR;

namespace ElectricityShop.Application.Features.Orders.Commands
{
    // Mirrors ProcessPaymentRequest from OrdersController
    public class ProcessPaymentCommand : IRequest<bool> // bool: true for success, false for failure
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; } // To ensure user is paying for their own order

        // Payment Details
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string CVV { get; set; }
        
        // Billing Address is now expected to be AddressDto from Queries namespace
        public AddressDto BillingAddress { get; set; }
    }
}
