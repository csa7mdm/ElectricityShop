using MediatR;
using System;
using System.Collections.Generic;

namespace ElectricityShop.Application.Features.Orders.Queries
{
    /// <summary>
    /// Query to get orders for a user
    /// </summary>
    public class GetOrdersQuery : IRequest<List<OrderDto>>
    {
        /// <summary>
        /// User ID to get orders for
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Optional filter by order status
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Optional start date filter
        /// </summary>
        public DateTime? StartDate { get; set; }
        
        /// <summary>
        /// Optional end date filter
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}