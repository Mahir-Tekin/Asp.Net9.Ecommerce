using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces.ServiceInterfaces;
using Asp.Net9.Ecommerce.Domain.Identity;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Authentication.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtService _jwtService;

        public RegisterCommandHandler(
            IIdentityService identityService,
            IJwtService jwtService)
        {
            _identityService = identityService;
            _jwtService = jwtService;
        }

        public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // 1. Create user
            var result = await _identityService.CreateUserAsync(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName);

            if (result.IsFailure)
            {
                return Result.Failure<AuthResponse>(result.Error);
            }

            // 2. Assign default Customer role
            var roleResult = await _identityService.AssignUserToRoleAsync(result.Value, AppRoles.Customer);
            if (roleResult.IsFailure)
            {
                return Result.Failure<AuthResponse>(roleResult.Error);
            }

            // 3. Generate tokens using the known Customer role
            var roles = new List<string> { AppRoles.Customer };
            var token = _jwtService.GenerateToken(result.Value, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var expiryTime = _jwtService.GetRefreshTokenExpiryTime();

            // 4. Store refresh token
            var refreshTokenResult = await _identityService.AddRefreshTokenAsync(
                result.Value,
                refreshToken,
                expiryTime);

            if (refreshTokenResult.IsFailure)
            {
                return Result.Failure<AuthResponse>(refreshTokenResult.Error);
            }

            // 5. Return full auth response
            var response = new AuthResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                UserId = result.Value,
                Email = request.Email,
                FullName = $"{request.FirstName} {request.LastName}",
                Roles = roles,
                RefreshTokenExpiryTime = expiryTime
            };

            return Result.Success(response);
        }
    }
} 