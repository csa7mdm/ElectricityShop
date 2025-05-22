using System;

namespace ElectricityShop.Application.Features.Carts.Queries
{
    public class CartItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } // Assuming you want to show product name in cart
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
