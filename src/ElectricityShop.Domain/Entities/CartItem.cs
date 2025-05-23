using System;

namespace ElectricityShop.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // Added this property
        public Guid CartId { get; set; }
        public Cart? Cart { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
    }
}