using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectricityShop.API.Models;
using ElectricityShop.Application.Features.Products.Commands;
using ElectricityShop.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricityShop.API.Controllers
{
    /// <summary>
    /// API controller for product operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets a paginated list of products with optional filtering
        /// </summary>
        /// <param name="paginationParams">Pagination parameters</param>
        /// <param name="query">Query parameters for filtering products</param>
        /// <returns>Paginated list of products</returns>
        [HttpGet]
        [ResponseCache(CacheProfileName = "Products")]
        public async Task<ActionResult<PagedResponse<ProductDto>>> GetProducts(
            [FromQuery] PaginationParams paginationParams,
            [FromQuery] GetProductsQuery query)
        {
            // Set pagination parameters on the query
            query.PageNumber = paginationParams.PageNumber;
            query.PageSize = paginationParams.PageSize;

            var result = await _mediator.Send(query);
            
            var pagedResponse = new PagedResponse<ProductDto>(
                result,
                query.TotalCount,
                query.PageNumber,
                query.PageSize);

            return pagedResponse;
        }

        /// <summary>
        /// Gets a specific product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        /// <response code="200">Product found</response>
        /// <response code="404">Product not found</response>
        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "ProductDetail")]
        public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
        {
            var query = new GetProductByIdQuery { ProductId = id };
            var productDto = await _mediator.Send(query);

            if (productDto == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            return Ok(productDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Guid>> CreateProduct([FromBody] CreateProductCommand command)
        {
            var productId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetProduct), new { id = productId }, productId);
        }

        /// <summary>
        /// Updates an existing product (admin only)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="command">Updated product data</param>
        /// <returns>No content on success</returns>
        /// <response code="204">Product updated successfully</response>
        /// <response code="400">Invalid product data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden - requires admin role</response>
        /// <response code="404">Product not found</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command)
        {
            command.Id = id; // Set the Id from the route parameter

            var success = await _mediator.Send(command);

            if (success)
            {
                return Ok(); // Or NoContent()
            }
            else
            {
                return NotFound($"Product with ID {id} not found or update failed.");
            }
        }

        /// <summary>
        /// Deletes a product (admin only)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>No content on success</returns>
        /// <response code="204">Product deleted successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden - requires admin role</response>
        /// <response code="404">Product not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            var command = new DeleteProductCommand { ProductId = id };
            var success = await _mediator.Send(command);

            if (success)
            {
                return NoContent(); // Successful deletion, no content to return
            }
            else
            {
                return NotFound($"Product with ID {id} not found.");
            }
        }
    }
}