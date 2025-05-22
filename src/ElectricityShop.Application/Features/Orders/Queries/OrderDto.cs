using System;
using System.Collections.Generic;

namespace ElectricityShop.Application.Features.Orders.Queries
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public AddressDto ShippingAddress { get; set; }
        // Add other relevant properties like BillingAddress, PaymentInfoSummary etc. if needed
    }
}
