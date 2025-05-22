using System;
using MediatR;

namespace ElectricityShop.Application.Features.Carts.Queries
{
    public class GetCartQuery : IRequest<CartDto>
    {
        public Guid UserId { get; set; }
    }
}
