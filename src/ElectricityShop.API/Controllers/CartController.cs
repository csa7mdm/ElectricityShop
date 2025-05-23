using System;
using System.Threading.Tasks;
using ElectricityShop.Application.Features.Carts.Queries;
using ElectricityShop.Application.Features.Carts.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElectricityShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userIdString = User.FindFirstValue("id"); // "id" claim as per OrdersController
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID claim is missing or invalid.");
            }

            var query = new GetCartQuery { UserId = userId };
            var cartDto = await _mediator.Send(query);

            if (cartDto == null)
            {
                return NotFound($"Cart not found for user {userId}");
            }

            return Ok(cartDto);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItemToCart([FromBody] AddCartItemRequest request)
        {
            var userIdString = User.FindFirstValue("id");
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID claim is missing or invalid.");
            }

            if (request.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            var command = new AddItemToCartCommand // Use simple name
            {
                UserId = userId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };

            await _mediator.Send(command);

            // Consider returning the updated cart or a specific success message/object
            // For now, Ok() indicates success.
            return Ok(); 
        }

        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateCartItem(Guid id, [FromBody] UpdateCartItemRequest request)
        {
            var userIdString = User.FindFirstValue("id");
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID claim is missing or invalid.");
            }

            if (request.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            var command = new UpdateCartItemCommand
            {
                UserId = userId,
                ProductId = id, // 'id' from route is ProductId
                NewQuantity = request.Quantity
            };

            var success = await _mediator.Send(command);

            if (success)
            {
                return Ok(); // Or NoContent()
            }
            else
            {
                // This could be due to item not found, or invalid quantity as per handler logic
                return NotFound($"Item with ProductId {id} not found in cart, or update failed."); 
            }
        }

        [HttpDelete("items/{id}")]
        public async Task<IActionResult> RemoveCartItem(Guid id)
        {
            var userIdString = User.FindFirstValue("id");
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID claim is missing or invalid.");
            }

            var command = new RemoveCartItemCommand
            {
                UserId = userId,
                ProductId = id // 'id' from route is ProductId
            };

            var success = await _mediator.Send(command);

            if (success)
            {
                return Ok(); // Or NoContent() for successful deletion without a body
            }
            else
            {
                return NotFound($"Item with ProductId {id} not found in cart.");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userIdString = User.FindFirstValue("id");
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID claim is missing or invalid.");
            }

            var command = new ClearCartCommand
            {
                UserId = userId
            };

            await _mediator.Send(command);

            return NoContent(); // Successful operation, no content to return
        }
    }

    public class AddCartItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartItemRequest
    {
        public int Quantity { get; set; }
    }
}