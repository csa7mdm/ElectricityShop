using System;
using MediatR;

namespace ElectricityShop.Application.Features.Products.Commands
{
    public class UpdateProductCommand : IRequest<bool> // bool: true if updated, false if not found/failed
    {
        public Guid Id { get; set; } // This will be set from the route parameter
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsActive { get; set; }
    }
}
