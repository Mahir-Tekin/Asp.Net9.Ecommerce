using Asp.Net9.Ecommerce.Application.Authentication.Commands.Login;
using Asp.Net9.Ecommerce.Application.Authentication.Commands.RefreshToken;
using Asp.Net9.Ecommerce.Application.Authentication.Commands.Register;
using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using Asp.Net9.Ecommerce.Application.Authentication.Queries.GetCurrentUser;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        /// Login with email and password
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>Authentication result with JWT token if successful</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var command = _mapper.Map<LoginCommand>(request);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="request">User registration details</param>
        /// <returns>Authentication result with JWT token if successful</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            var command = _mapper.Map<RegisterCommand>(request);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        /// <summary>
        /// Refresh an expired access token using a refresh token
        /// </summary>
        /// <param name="request">The current access token and refresh token</param>
        /// <returns>New access token and refresh token if successful</returns>
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenCommand request)
        {
            var result = await _mediator.Send(request);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        /// <summary>
        /// Get current user information
        /// </summary>
        /// <returns>Current user details if authenticated</returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserProfileResponse>> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = new GetCurrentUserQuery(userId);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
} 