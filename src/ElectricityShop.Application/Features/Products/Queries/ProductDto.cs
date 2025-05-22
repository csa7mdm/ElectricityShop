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
        
        /// <summary>
        /// Name of the category
        /// </summary>
        public string CategoryName { get; set; }
        
        /// <summary>
        /// Whether the product is active/available
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// List of image URLs for the product
        /// </summary>
        public List<string> ImageUrls { get; set; }
        
        /// <summary>
        /// Average rating for the product (1-5)
        /// </summary>
        public decimal? AverageRating { get; set; }
        
        /// <summary>
        /// Number of reviews for the product
        /// </summary>
        public int ReviewCount { get; set; }
        
        /// <summary>
        /// Whether the product is on sale
        /// </summary>
        public bool OnSale { get; set; }
        
        /// <summary>
        /// Discount percentage if on sale
        /// </summary>
        public decimal? DiscountPercentage { get; set; }
    }
}