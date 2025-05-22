using System;
using MediatR;

namespace ElectricityShop.Application.Features.Carts.Commands
{
    public class ClearCartCommand : IRequest<Unit> // Unit because the operation doesn't need to return a value
    {
        public Guid UserId { get; set; }
    }
}
