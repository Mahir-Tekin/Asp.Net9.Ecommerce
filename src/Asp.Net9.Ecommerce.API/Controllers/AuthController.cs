using Asp.Net9.Ecommerce.Application.Authentication.Commands.Register;
using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Asp.Net9.Ecommerce.API.Controllers
{
    /// <summary>
    /// Controller for handling authentication operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AuthController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="request">User registration details</param>
        /// <returns>Authentication response with JWT tokens if successful</returns>
        /// <response code="200">Returns the authentication response with tokens</response>
        /// <response code="400">If the registration request is invalid or fails</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var command = _mapper.Map<RegisterCommand>(request);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
} 