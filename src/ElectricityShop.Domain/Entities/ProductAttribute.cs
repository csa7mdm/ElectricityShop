using System;

namespace ElectricityShop.Domain.Entities
{
    public class ProductAttribute : BaseEntity
    {
        public required string Name { get; set; }
        public required string Value { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
    }
}