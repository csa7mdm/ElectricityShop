using System;
using MediatR;

namespace ElectricityShop.Application.Features.Carts.Commands
{
    public class UpdateCartItemCommand : IRequest<bool> // Returning bool: true if updated, false if not found
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; } // Assuming 'id' from route is ProductId
        public int NewQuantity { get; set; }
    }
}
