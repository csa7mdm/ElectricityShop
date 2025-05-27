using System;
using System.Collections.Generic;

namespace ElectricityShop.Application.Features.Products.Queries
{
    /// <summary>
    /// Data transfer object for product data
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Unique identifier for the product
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Name of the product
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Detailed description of the product
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Product price
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// Number of items in stock
        /// </summary>
        public int StockQuantity { get; set; }
        
        /// <summary>
        /// Category the product belongs to
        /// </summary>
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } // Assuming you want to show category name
        public bool IsActive { get; set; }
        public List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
        public List<ProductAttributeDto> Attributes { get; set; } = new List<ProductAttributeDto>();
    }
}
