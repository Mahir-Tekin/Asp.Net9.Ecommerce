using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Authentication.Commands.Register
{
    public record RegisterCommand : IRequest<Result<AuthResponse>>
    {
        public string Email { get; init; }
        public string Password { get; init; }
        public string ConfirmPassword { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
    }
} 