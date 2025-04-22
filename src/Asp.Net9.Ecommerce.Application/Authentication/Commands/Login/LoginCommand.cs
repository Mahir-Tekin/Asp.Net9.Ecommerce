using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Authentication.Commands.Login
{
    /// <summary>
    /// Command to handle user login
    /// </summary>
    public record LoginCommand : IRequest<Result<AuthResponse>>
    {
        /// <summary>
        /// User's email address
        /// </summary>
        public string Email { get; init; }

        /// <summary>
        /// User's password
        /// </summary>
        public string Password { get; init; }

        /// <summary>
        /// Whether to extend the refresh token validity period
        /// </summary>
        public bool RememberMe { get; init; }
    }
} 