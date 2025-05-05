using System.IdentityModel.Tokens.Jwt;
using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces.ServiceInterfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Authentication.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtService _jwtService;

        public RefreshTokenCommandHandler(
            IIdentityService identityService,
            IJwtService jwtService)
        {
            _identityService = identityService;
            _jwtService = jwtService;
        }

        public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // 1. Extract user ID from the expired access token
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(request.AccessToken);
            var userId = jwtToken.Subject;

            if (string.IsNullOrEmpty(userId))
                return Result.Failure<AuthResponse>(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Token", "Invalid access token") }));

            // 2. Get and validate the refresh token
            var refreshTokenResult = await _identityService.GetActiveRefreshTokenAsync(userId, request.RefreshToken);
            if (refreshTokenResult.IsFailure)
                return Result.Failure<AuthResponse>(refreshTokenResult.Error);

            // 3. Get user roles
            var rolesResult = await _identityService.GetUserRolesAsync(userId);
            if (rolesResult.IsFailure)
                return Result.Failure<AuthResponse>(rolesResult.Error);

            // 4. Generate new tokens
            var newAccessToken = _jwtService.GenerateToken(userId, rolesResult.Value);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // Keep the same expiry time as the original token
            var expiryTime = refreshTokenResult.Value.ExpiresOnUtc;

            // 5. Replace the old refresh token
            var replaceResult = await _identityService.ReplaceRefreshTokenAsync(
                userId,
                request.RefreshToken,
                newRefreshToken,
                expiryTime);

            if (replaceResult.IsFailure)
                return Result.Failure<AuthResponse>(replaceResult.Error);

            // 6. Get user details for the response
            var userDetailsResult = await _identityService.GetUserDetailsAsync(userId);
            if (userDetailsResult.IsFailure)
                return Result.Failure<AuthResponse>(userDetailsResult.Error);

            (string firstName, string lastName, string email) = userDetailsResult.Value;

            // 7. Return the new tokens
            var response = new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                UserId = userId,
                Email = email,
                FullName = $"{firstName} {lastName}".Trim(),
                Roles = rolesResult.Value.ToList(),
                RefreshTokenExpiryTime = expiryTime
            };

            return Result.Success(response);
        }
    }
} 