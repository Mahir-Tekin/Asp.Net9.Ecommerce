using Asp.Net9.Ecommerce.API.Extensions;
using Asp.Net9.Ecommerce.Application.Authentication.Commands.Address;
using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Asp.Net9.Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AddressesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AddressesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Adds a new address for the current user
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAddressRequest request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
                return Unauthorized();

            var command = _mapper.Map<CreateAddressCommand>(request);
            command.UserId = parsedUserId;

            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsSuccess)
                return CreatedAtAction(nameof(Create), new { id = result.Value }, result.Value);
            return result.ToActionResult();
        }

        /// <summary>
        /// Gets all addresses for the current user
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<AddressResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
                return Unauthorized();

            var query = new Asp.Net9.Ecommerce.Application.Authentication.Queries.Address.GetAddressesByUserIdQuery(parsedUserId);
            var result = await _mediator.Send(query, cancellationToken);
            if (result.IsSuccess)
                return Ok(result.Value);
            return result.ToActionResult();
        }
    }
}
