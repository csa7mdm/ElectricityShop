using System;

namespace ElectricityShop.Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        public required string ImageUrl { get; set; }
        public bool IsMain { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
    }
}