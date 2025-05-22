using System;
using MediatR;

namespace ElectricityShop.Application.Features.Carts.Commands
{
    public class RemoveCartItemCommand : IRequest<bool> // bool: true if removed, false if not found
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; } // 'id' from route, treated as ProductId of the item to remove
    }
}
