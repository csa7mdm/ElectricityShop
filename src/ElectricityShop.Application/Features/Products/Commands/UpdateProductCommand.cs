using System;
using System.Collections.Generic; // Added
using ElectricityShop.Application.Features.Products.Commands.Dtos; // Added
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
        public required string Name { get; set; } // Added required
        public string? Description { get; set; } // Made nullable
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

        public List<ProductImageUpdateDto> Images { get; set; } = new List<ProductImageUpdateDto>(); // Added
        public List<ProductAttributeUpdateDto> Attributes { get; set; } = new List<ProductAttributeUpdateDto>(); // Added
    }
}
