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
            // Validate credentials
            var validationResult = await _identityService.ValidateCredentialsAsync(request.Email, request.Password);
            if (validationResult.IsFailure)
                return Result.Failure<AuthResponse>(validationResult.Error);

            var (userId, user) = validationResult.Value;

            // Get user roles
            var rolesResult = await _identityService.GetUserRolesAsync(userId);
            if (rolesResult.IsFailure)
                return Result.Failure<AuthResponse>(rolesResult.Error);

            // Generate tokens
            var accessToken = _jwtService.GenerateToken(userId, rolesResult.Value);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshTokenExpiry = _jwtService.GetRefreshTokenExpiryTime();
            
            // Update refresh token
            var updateResult = await _identityService.UpdateRefreshTokenAsync(userId, refreshToken, refreshTokenExpiry);
            if (updateResult.IsFailure)
                return Result.Failure<AuthResponse>(updateResult.Error);

            // Return successful response
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