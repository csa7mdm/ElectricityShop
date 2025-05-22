using System;
using System.Collections.Generic;

namespace ElectricityShop.Application.Features.Carts.Queries
{
    public class CartDto
    {
        public Guid CartId { get; set; }
        public Guid UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal TotalAmount => Items.Sum(item => item.TotalPrice);
    }
}
