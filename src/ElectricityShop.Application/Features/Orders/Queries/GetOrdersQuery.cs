using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Domain.Enums;
using ElectricityShop.Domain.Interfaces;
using MediatR;

namespace ElectricityShop.Application.Features.Orders.Queries
{
    public class GetOrdersQuery : IRequest<List<OrderDto>>
    {
        public Guid UserId { get; set; }
        public OrderStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<OrderDto>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetOrdersQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
            {
                var orders = await _unitOfWork.Orders.ListAsync(o => o.UserId == request.UserId);

                // Filter by status
                if (request.Status.HasValue)
                {
                    orders = orders.Where(o => o.Status == request.Status.Value).ToList();
                }

                // Filter by date range
                if (request.FromDate.HasValue)
                {
                    orders = orders.Where(o => o.CreatedAt >= request.FromDate.Value).ToList();
                }

                if (request.ToDate.HasValue)
                {
                    orders = orders.Where(o => o.CreatedAt <= request.ToDate.Value).ToList();
                }

                // Sort by date descending (newest first)
                orders = orders.OrderByDescending(o => o.CreatedAt).ToList();

                var orderDtos = new List<OrderDto>();

                foreach (var order in orders)
                {
                    var orderItems = await _unitOfWork.OrderItems.ListAsync(oi => oi.OrderId == order.Id);
                    
                    var orderItemDtos = orderItems.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        TotalPrice = oi.Quantity * oi.UnitPrice
                    }).ToList();

                    orderDtos.Add(new OrderDto
                    {
                        Id = order.Id,
                        OrderNumber = order.OrderNumber,
                        Status = order.Status,
                        TotalAmount = order.TotalAmount,
                        PaymentMethod = order.PaymentMethod,
                        IsPaid = order.IsPaid,
                        PaidAt = order.PaidAt,
                        ShippingAddress = order.ShippingAddress,
                        BillingAddress = order.BillingAddress,
                        CreatedAt = order.CreatedAt,
                        Items = orderItemDtos
                    });
                }

                return orderDtos;
            }
        }
    }

    public class OrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }

    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}