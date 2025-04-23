using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using MediatR;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Application.Authentication.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(
            IIdentityService identityService,
            IJwtService jwtService)
        {
            _identityService = identityService;
            _jwtService = jwtService;
        }

        public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // 1. Find user and validate credentials
            var validationResult = await _identityService.ValidateCredentialsAsync(request.Email, request.Password);
            if (validationResult.IsFailure)
                return Result.Failure<AuthResponse>(validationResult.Error);

            var (userId, user) = validationResult.Value;

            // 2. Check if user is locked out
            var lockoutResult = await _identityService.IsLockedOutAsync(userId);
            if (lockoutResult.IsFailure)
                return Result.Failure<AuthResponse>(lockoutResult.Error);

            if (lockoutResult.Value)
                return Result.Failure<AuthResponse>(ErrorResponse.General(
                    "Account is locked out. Please try again later.",
                    "ACCOUNT_LOCKED"));

            // 3. Get user roles
            var rolesResult = await _identityService.GetUserRolesAsync(userId);
            if (rolesResult.IsFailure)
                return Result.Failure<AuthResponse>(rolesResult.Error);

            // 4. Generate tokens
            var accessToken = _jwtService.GenerateToken(userId, rolesResult.Value);
            var refreshToken = _jwtService.GenerateRefreshToken();
            
            // Calculate expiry time based on RememberMe
            var refreshTokenExpiry = request.RememberMe 
                ? _jwtService.GetExtendedRefreshTokenExpiryTime()
                : _jwtService.GetRefreshTokenExpiryTime();
            
            // 5. Store refresh token
            var refreshTokenResult = await _identityService.AddRefreshTokenAsync(
                userId, 
                refreshToken, 
                refreshTokenExpiry);

            if (refreshTokenResult.IsFailure)
                return Result.Failure<AuthResponse>(refreshTokenResult.Error);

            // 6. Return successful response
            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = userId,
                Email = user.Email,
                FullName = user.FullName ?? string.Empty,
                Roles = rolesResult.Value.ToList(),
                RefreshTokenExpiryTime = refreshTokenExpiry
            };

            return Result.Success(response);
        }
    }
} 