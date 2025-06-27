using Asp.Net9.Ecommerce.Application.Orders.Commands;
using Asp.Net9.Ecommerce.Application.Orders.DTOs;
using Asp.Net9.Ecommerce.Application.Orders.Queries;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Asp.Net9.Ecommerce.API.Extensions;

namespace Asp.Net9.Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="dto">The order data transfer object containing order details.</param>
        /// <returns>Returns the ID of the created order.</returns>
        /// <response code="201">Order created successfully.</response>
        /// <response code="400">Invalid order data.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            var command = new CreateOrderCommand(dto, userId);
            var orderId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = orderId }, new { id = orderId });
        }

        /// <summary>
        /// Gets the current user's orders (paginated).
        /// </summary>
        /// <param name="page">Page number (default 1)</param>
        /// <param name="pageSize">Page size (default 10)</param>
        /// <returns>Paginated list of user's orders</returns>
        [HttpGet("my")]
        [ProducesResponseType(typeof(List<OrderSummaryDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetMyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            var query = new GetUserOrdersQuery
            {
                UserId = userId,
                Page = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Gets an order by ID for the current user.
        /// </summary>
        /// <param name="id">The ID of the order.</param>
        /// <returns>Returns the order details.</returns>
        /// <response code="200">Order retrieved successfully.</response>
        /// <response code="404">Order not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderSummaryDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            var query = new GetUserOrderByIdQuery
            {
                UserId = userId,
                OrderId = id
            };
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Gets a paginated list of all orders for admin.
        /// </summary>
        /// <param name="page">Page number (default 1)</param>
        /// <param name="pageSize">Page size (default 10)</param>
        /// <param name="status">Order status filter (optional)</param>
        /// <param name="sortAsc">Sort by created date ascending (default false)</param>
        /// <returns>Paginated list of all orders</returns>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AdminOrderListResultDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetAdminOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null, [FromQuery] bool sortAsc = false)
        {
            var query = new GetAdminOrdersQuery
            {
                Page = page,
                PageSize = pageSize,
                Status = status,
                SortAsc = sortAsc
            };
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Gets order details by ID for admin.
        /// </summary>
        /// <param name="id">The ID of the order.</param>
        /// <returns>Returns the order details.</returns>
        /// <response code="200">Order retrieved successfully.</response>
        /// <response code="404">Order not found.</response>
        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(OrderSummaryDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> GetAdminOrderById(Guid id)
        {
            var query = new GetAdminOrderByIdQuery { OrderId = id };
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Updates the status of an order (admin only).
        /// </summary>
        /// <param name="id">The ID of the order.</param>
        /// <param name="dto">The new status and optional comment.</param>
        /// <returns>Result of the update operation.</returns>
        /// <response code="200">Order status updated successfully.</response>
        /// <response code="404">Order not found.</response>
        [HttpPut("admin/{id}/status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
        {
            var command = new UpdateOrderStatusCommand
            {
                OrderId = id,
                NewStatus = dto.NewStatus,
                Comment = dto.Comment
            };
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }
    }
}
