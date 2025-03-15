using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectricityShop.Application.Features.Orders.Commands;
using ElectricityShop.Application.Features.Orders.Queries;
using ElectricityShop.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricityShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetOrders([FromQuery] GetOrdersQuery query)
        {
            // Get current user ID from claims
            var userId = Guid.Parse(User.FindFirst("id")?.Value);
            query.UserId = userId;

            return await _mediator.Send(query);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
        {
            // You would implement a GetOrderByIdQuery
            // For now, we'll just return a 501 Not Implemented
            return StatusCode(501);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateOrder([FromBody] CreateOrderCommand command)
        {
            // Get current user ID from claims
            var userId = Guid.Parse(User.FindFirst("id")?.Value);
            command.UserId = userId;

            var orderId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetOrder), new { id = orderId }, orderId);
        }

        [HttpPut("{id}/cancel")]
        public async Task<ActionResult> CancelOrder(Guid id)
        {
            // You would implement a CancelOrderCommand
            // For now, we'll just return a 501 Not Implemented
            return StatusCode(501);
        }

        [HttpPost("{id}/pay")]
        public async Task<ActionResult> ProcessPayment(Guid id, [FromBody] ProcessPaymentRequest request)
        {
            // You would implement a ProcessPaymentCommand
            // For now, we'll just return a 501 Not Implemented
            return StatusCode(501);
        }
    }

    public class ProcessPaymentRequest
    {
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string CVV { get; set; }
        public BillingAddressDto BillingAddress { get; set; }
    }

    public class BillingAddressDto
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }
}