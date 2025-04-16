using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using MediatR;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Application.Authentication.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
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

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Validate credentials
            var validationResult = await _identityService.ValidateCredentialsAsync(request.Email, request.Password);
            if (validationResult.IsFailure)
                return AuthResponse.Failure(new[] { validationResult.Error });

            var (userId, user) = validationResult.Value;

            // Get user roles
            var roles = await _identityService.GetUserRolesAsync(userId);

            // Generate tokens
            var accessToken = _jwtService.GenerateToken(userId, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshTokenExpiry = _jwtService.GetRefreshTokenExpiryTime();
            
            // Update refresh token
            await _identityService.UpdateRefreshTokenAsync(userId, refreshToken, refreshTokenExpiry);

            // Return successful response
            return AuthResponse.Success(
                accessToken,
                refreshToken,
                userId,
                user.Email,
                user.FullName ?? string.Empty,
                roles,
                refreshTokenExpiry);
        }
    }
} 