using MediatR;
using System;

namespace ElectricityShop.Application.Features.Products.Commands
{
    /// <summary>
    /// Command to create a new product
    /// </summary>
    public class CreateProductCommand : IRequest<Guid>
    {
        /// <summary>
        /// Name of the product
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Description of the product
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Price of the product
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// Initial stock quantity
        /// </summary>
        public int StockQuantity { get; set; }
        
        /// <summary>
        /// Category ID
        /// </summary>
        public Guid CategoryId { get; set; }
    }
}