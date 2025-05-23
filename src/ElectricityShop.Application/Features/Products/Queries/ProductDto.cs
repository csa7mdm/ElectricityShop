using System;

namespace ElectricityShop.Application.Features.Products.Queries
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } // Assuming you want to show category name
        public bool IsActive { get; set; }
        public List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
        public List<ProductAttributeDto> Attributes { get; set; } = new List<ProductAttributeDto>();
    }
}
