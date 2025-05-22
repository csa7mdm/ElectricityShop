using MediatR;
using System;

namespace ElectricityShop.Application.Features.Products.Commands
{
    /// <summary>
    /// Command to update an existing product
    /// </summary>
    public class UpdateProductCommand : IRequest<bool>
    {
        /// <summary>
        /// ID of the product to update
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Updated name of the product
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Updated description of the product
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Updated price of the product
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// Updated stock quantity
        /// </summary>
        public int StockQuantity { get; set; }
        
        /// <summary>
        /// Updated category ID
        /// </summary>
        public Guid CategoryId { get; set; }
        
        /// <summary>
        /// Whether the product is active
        /// </summary>
        public bool IsActive { get; set; }
    }
}