using System;

namespace ElectricityShop.Application.Features.Carts.Queries
{
    public class CartItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        public string? ProductImageUrl { get; set; } // Added this property
    }
}
