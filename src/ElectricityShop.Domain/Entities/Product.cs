using System;

namespace ElectricityShop.Domain.Entities
{
    /// <summary>
    /// Represents a product
    /// </summary>
    public class Product : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the product description
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Gets or sets the product price
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// Gets or sets the product category ID
        /// </summary>
        public Guid CategoryId { get; set; }
        
        /// <summary>
        /// Gets or sets the product brand ID
        /// </summary>
        public Guid BrandId { get; set; }
        
        /// <summary>
        /// Gets or sets the product stock quantity
        /// </summary>
        public int StockQuantity { get; set; }
        
        /// <summary>
        /// Gets or sets the product image URL
        /// </summary>
        public string ImageUrl { get; set; }
    }
}