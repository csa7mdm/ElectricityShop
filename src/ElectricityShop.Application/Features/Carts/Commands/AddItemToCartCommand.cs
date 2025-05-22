using System;
using MediatR;

namespace ElectricityShop.Application.Features.Carts.Commands
{
    public class AddItemToCartCommand : IRequest<Unit> // Using Unit for commands that don't return a value
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
