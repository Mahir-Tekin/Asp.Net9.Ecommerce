using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Authentication.Commands.Login
{
    public record LoginCommand : IRequest<Result<AuthResponse>>
    {
        public string Email { get; init; }
        public string Password { get; init; }
        public bool RememberMe { get; init; }
    }
} 