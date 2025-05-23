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
            var userIdString = User.FindFirst("id")?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID claim is missing or invalid.");
            }

            var query = new GetOrderByIdQuery { OrderId = id, UserId = userId };
            var orderDto = await _mediator.Send(query);

            if (orderDto == null)
            {
                return NotFound($"Order with ID {id} not found for the current user.");
            }

            return Ok(orderDto);
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
            var userIdString = User.FindFirst("id")?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID claim is missing or invalid.");
            }

            var command = new CancelOrderCommand { OrderId = id, UserId = userId };
            var success = await _mediator.Send(command);

            if (success)
            {
                return Ok(); // Or NoContent()
            }
            else
            {
                // This could be due to order not found, not belonging to user, or not being in a cancelable state
                return NotFound($"Order with ID {id} not found or cannot be canceled by the current user.");
            }
        }

        [HttpPost("{id}/pay")]
        public async Task<ActionResult> ProcessPayment(Guid id, [FromBody] ProcessPaymentRequest request)
        {
            var userIdString = User.FindFirst("id")?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID claim is missing or invalid.");
            }

            var command = new ProcessPaymentCommand
            {
                OrderId = id,
                UserId = userId,
                CardNumber = request.CardNumber,
                CardHolderName = request.CardHolderName,
                ExpiryMonth = request.ExpiryMonth,
                ExpiryYear = request.ExpiryYear,
                CVV = request.CVV,
                BillingAddress = new Application.Features.Orders.Queries.AddressDto // Explicitly map
                {
                    Street = request.BillingAddress.AddressLine1, // Assuming AddressLine1 maps to Street
                    City = request.BillingAddress.City,
                    State = request.BillingAddress.State,
                    ZipCode = request.BillingAddress.ZipCode,
                    Country = request.BillingAddress.Country
                    // Note: AddressLine2 is not in AddressDto, can be added if needed
                }
            };

            var success = await _mediator.Send(command);

            if (success)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Payment failed or order cannot be processed.");
            }
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