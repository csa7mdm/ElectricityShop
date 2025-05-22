using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            try
            {
                var order = await _orderService.GetOrderAsync(id, cancellationToken);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order {OrderId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
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

        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateOrderStatus(
            Guid id, 
            UpdateOrderStatusRequest request, 
            CancellationToken cancellationToken)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(
                    id,
                    request.NewStatus,
                    request.UpdatedById,
                    request.Notes,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for order {OrderId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> CancelOrder(
            Guid id, 
            CancelOrderRequest request, 
            CancellationToken cancellationToken)
        {
            try
            {
                await _orderService.CancelOrderAsync(
                    id,
                    request.Reason,
                    request.CancelledById,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }

    public class CreateOrderRequest
    {
        public Guid CustomerId { get; set; }
        public List<OrderItemDto> Items { get; set; }
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