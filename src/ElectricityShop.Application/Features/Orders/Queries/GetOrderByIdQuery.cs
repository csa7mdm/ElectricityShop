using System;
using MediatR;

namespace ElectricityShop.Application.Features.Orders.Queries
{
    public class GetOrderByIdQuery : IRequest<OrderDto> // Returns OrderDto or null
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; } // To ensure user can only access their own orders
    }
}
