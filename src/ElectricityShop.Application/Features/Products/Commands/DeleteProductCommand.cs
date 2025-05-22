using MediatR;
using System;

namespace ElectricityShop.Application.Features.Products.Commands
{
    /// <summary>
    /// Command to delete an existing product
    /// </summary>
    public class DeleteProductCommand : IRequest<bool>
    {
        /// <summary>
        /// ID of the product to delete
        /// </summary>
        public Guid Id { get; set; }
    }
}