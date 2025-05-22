using MediatR;
using System;

namespace ElectricityShop.Application.Features.Products.Queries
{
    /// <summary>
    /// Query to get a product by its ID
    /// </summary>
    public class GetProductByIdQuery : IRequest<ProductDto>
    {
        /// <summary>
        /// ID of the product to retrieve
        /// </summary>
        public Guid Id { get; set; }
    }
}