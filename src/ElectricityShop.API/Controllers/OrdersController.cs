using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Application.Features.Orders.Commands;
using ElectricityShop.Application.Features.Orders.Queries; // Added for GetOrderByIdQuery, OrderDto
using MediatR; // Added for IMediator
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic; // Added for List<T>

namespace ElectricityShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMediator _mediator; // Added IMediator field

        public OrdersController(
            IOrderService orderService,
            ILogger<OrdersController> logger,
            IMediator mediator) // Added IMediator to constructor
        {
            _orderService = orderService;
            _logger = logger;
            _mediator = mediator; // Assigned IMediator
        }

        // Removed duplicate GetOrder method.
        // The one below is more complete.

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
        {
            var userIdString = User.FindFirst("id")?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID claim is missing or invalid.");
            }

            var query = new GetOrderByIdQuery { OrderId = id, UserId = userId };
            // Assuming OrderDto is resolved via ElectricityShop.Application.Features.Orders.Queries
            var orderDto = await _mediator.Send(query);

            if (orderDto == null)
            {
                return NotFound($"Order with ID {id} not found for the current user.");
            }

            return Ok(orderDto);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Assuming CreateOrderCommand is the target for MediatR if IOrderService is removed.
                // For now, IOrderService.CreateOrderAsync is called.
                // If CreateOrderCommand is to be used directly:
                // var command = new CreateOrderCommand { CustomerId = request.CustomerId, Items = request.Items /* map appropriately */ };
                // var orderId = await _mediator.Send(command, cancellationToken);
                // return Ok(orderId);

                // Mapping API DTO Items to Application DTO Items
                var applicationItems = new List<ElectricityShop.Application.Features.Orders.Queries.OrderItemDto>();
                if (request.Items != null)
                {
                    foreach (var apiItem in request.Items)
                    {
                        applicationItems.Add(new ElectricityShop.Application.Features.Orders.Queries.OrderItemDto
                        {
                            ProductId = apiItem.ProductId,
                            Quantity = apiItem.Quantity,
                            Price = apiItem.Price,
                            // ProductName might be set by the application/domain layer
                        });
                    }
                }
                
                var orderId = await _orderService.CreateOrderAsync(
                    request.CustomerId,
                    applicationItems, // Pass the mapped list
                    cancellationToken);

                return Ok(orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for customer {CustomerId}", request.CustomerId);
                return StatusCode(500, "An error occurred while processing your request");
            }
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
                // ProcessPaymentCommand, according to its 'using' directive, expects a DTO from 
                // ElectricityShop.Application.Features.Orders.Queries.
                // Assuming 'AddressDto' is the intended target in that namespace as per subtask hint.
                BillingAddress = new ElectricityShop.Application.Features.Orders.Queries.AddressDto 
                {
                    Street = request.BillingAddress.AddressLine1,
                    // AddressLine2 = request.BillingAddress.AddressLine2, // Add if Application's AddressDto has this
                    City = request.BillingAddress.City,
                    State = request.BillingAddress.State,
                    ZipCode = request.BillingAddress.ZipCode,
                    Country = request.BillingAddress.Country
                    // Ensure all mapped properties exist in Application.Features.Orders.Queries.AddressDto
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

    // public class UpdateOrderStatusRequest // This was present, ensure it's still needed or move/remove
    // {
    //     public string NewStatus { get; set; }
    //     public Guid? UpdatedById { get; set; }
    //     public string Notes { get; set; }
    // }

    // public class CancelOrderRequest // This was present, ensure it's still needed or move/remove
    // {
    //     public string Reason { get; set; }
    //     public Guid? CancelledById { get; set; }
    // }

    // Define API-specific request DTOs
    public class CreateOrderRequest
    {
        public Guid CustomerId { get; set; }
        public List<OrderItemDto> Items { get; set; } // Uses API-level OrderItemDto, defined below
    }

    public class OrderItemDto // API-level DTO for CreateOrderRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        // ProductName can be omitted here if it's not part of the request payload
        // and is instead looked up or set by the backend.
        // If it IS part of the request, uncomment:
        // public string ProductName { get; set; } 
    }

    public class ProcessPaymentRequest
    {
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string CVV { get; set; }
        public BillingAddressDto BillingAddress { get; set; } // API-level BillingAddressDto
    }

    public class BillingAddressDto // API-level DTO for ProcessPaymentRequest
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; } // Added AddressLine2 for completeness
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }
}