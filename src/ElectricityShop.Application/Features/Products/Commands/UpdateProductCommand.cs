using System;
using System.Collections.Generic; // Added
using ElectricityShop.Application.Features.Products.Commands.Dtos; // Added
using MediatR;

namespace ElectricityShop.Application.Features.Products.Commands
{
    public class UpdateProductCommand : IRequest<bool> 
    {
        public Guid Id { get; set; } 
        public required string Name { get; set; } // Added required
        public string? Description { get; set; } // Made nullable
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsActive { get; set; }

        public List<ProductImageUpdateDto> Images { get; set; } = new List<ProductImageUpdateDto>(); // Added
        public List<ProductAttributeUpdateDto> Attributes { get; set; } = new List<ProductAttributeUpdateDto>(); // Added
    }
}
