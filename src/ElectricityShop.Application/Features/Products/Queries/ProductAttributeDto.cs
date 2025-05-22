using System;

namespace ElectricityShop.Application.Features.Products.Queries
{
    public class ProductAttributeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
