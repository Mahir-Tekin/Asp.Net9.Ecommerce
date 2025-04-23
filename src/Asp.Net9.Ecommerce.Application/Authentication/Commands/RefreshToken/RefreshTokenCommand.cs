using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Authentication.Commands.RefreshToken
{
    /// <summary>
    /// Command to refresh an access token using a refresh token
    /// </summary>
    public record RefreshTokenCommand : IRequest<Result<AuthResponse>>
    {
        /// <summary>
        /// The current access token
        /// </summary>
        public string AccessToken { get; init; }

        /// <summary>
        /// The refresh token to use
        /// </summary>
        public string RefreshToken { get; init; }
    }
} 