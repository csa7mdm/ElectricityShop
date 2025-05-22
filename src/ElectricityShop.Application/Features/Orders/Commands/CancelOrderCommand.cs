using System;
using MediatR;

namespace ElectricityShop.Application.Features.Orders.Commands
{
    public class CancelOrderCommand : IRequest<bool> // bool: true if canceled, false if not found/not allowed
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; } // To ensure user can only cancel their own orders
    }
}
