using System;
using MediatR;
using System;

namespace ElectricityShop.Application.Features.Products.Commands
{
    public class DeleteProductCommand : IRequest<bool> // bool: true if deleted, false if not found
    {
        public Guid ProductId { get; set; }
    }
}
