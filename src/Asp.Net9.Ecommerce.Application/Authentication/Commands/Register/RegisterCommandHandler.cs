using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces;
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

            // 3. Get user roles (should now include Customer role)
            var roles = await _identityService.GetUserRolesAsync(result.Value);

            // 4. Generate tokens
            var token = _jwtService.GenerateToken(result.Value, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var expiryTime = _jwtService.GetRefreshTokenExpiryTime();

            // 5. Store refresh token
            await _identityService.UpdateRefreshTokenAsync(
                result.Value,
                refreshToken,
                expiryTime);

            // 6. Return full auth response
            var response = AuthResponse.Success(
                token,
                refreshToken,
                result.Value,
                request.Email,
                $"{request.FirstName} {request.LastName}",
                roles.ToList(),
                expiryTime);

            return Result.Success(response);
        }
    }
} 