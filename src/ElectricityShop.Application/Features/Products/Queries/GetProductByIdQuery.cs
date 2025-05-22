using System;
using MediatR;

namespace ElectricityShop.Application.Features.Products.Queries
{
    public class GetProductByIdQuery : IRequest<ProductDto> // Returns ProductDto or null
    {
        public Guid ProductId { get; set; }
    }
}
