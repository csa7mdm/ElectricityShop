using System;
using System.Collections.Generic;
using System.Linq; // Required for Enumerable.Sum

namespace ElectricityShop.Application.Features.Carts.Queries
{
    public class CartDto
    {
        public Guid Id { get; set; } // Renamed from CartId
        public Guid UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        
        // Calculated property for the sum of TotalPrice for all CartItemDtos
        public decimal GrandTotal => Items.Sum(item => item.TotalPrice); 
        
        // Calculated property for the sum of Quantity for all CartItemDtos
        public int TotalItems => Items.Sum(item => item.Quantity);
    }
}
