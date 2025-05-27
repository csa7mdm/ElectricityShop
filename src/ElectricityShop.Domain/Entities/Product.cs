using System;
using System.Collections.Generic; // Ensure this is present

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
        public Guid CategoryId { get; set; } // Assuming Category entity exists
        public virtual Category? Category { get; set; } // Navigation property
        
        /// <summary>
        /// Gets or sets the product brand ID
        /// </summary>
        public Guid BrandId { get; set; } // Assuming Brand entity exists
        // public virtual Brand Brand { get; set; } // Optional: Navigation property
        
        /// <summary>
        /// Gets or sets the product stock quantity
        /// </summary>
        public int StockQuantity { get; set; }
        
        // ImageUrl property removed

        /// <summary>
        /// Gets or sets the collection of product images.
        /// </summary>
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

        /// <summary>
        /// Gets or sets a value indicating whether the product is active.
        /// Defaults to true.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the collection of product attributes.
        /// </summary>
        public ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
    }
}