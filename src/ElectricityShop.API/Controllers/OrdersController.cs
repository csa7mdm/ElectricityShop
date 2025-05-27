using ElectricityShop.Application.Common.Interfaces;
using ElectricityShop.Application.Features.Orders.Commands;
using Microsoft.AspNetCore.Mvc;

namespace ElectricityShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            IOrderService orderService,
            ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid id, CancellationToken cancellationToken)
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
        public async Task<ActionResult<Guid>> CreateOrder(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var orderId = await _orderService.CreateOrderAsync(
                    request.CustomerId,
                    request.Items,
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

    public class UpdateOrderStatusRequest
    {
        public string NewStatus { get; set; }
        public Guid? UpdatedById { get; set; }
        public string Notes { get; set; }
    }

    public class CancelOrderRequest
    {
        public string Reason { get; set; }
        public Guid? CancelledById { get; set; }
    }
}