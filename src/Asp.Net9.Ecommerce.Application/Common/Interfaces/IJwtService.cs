using Asp.Net9.Ecommerce.Domain.Identity;

namespace Asp.Net9.Ecommerce.Application.Common.Interfaces
{
    public interface IJwtService
    {
        /// <summary>
        /// Generates a JWT access token for the user
        /// </summary>
        string GenerateToken(string userId, IEnumerable<string> roles);

        /// <summary>
        /// Generates a cryptographically secure refresh token
        /// </summary>
        string GenerateRefreshToken();

        /// <summary>
        /// Gets the standard refresh token expiry time
        /// </summary>
        DateTime GetRefreshTokenExpiryTime();

        /// <summary>
        /// Gets an extended refresh token expiry time for "Remember Me" functionality
        /// </summary>
        DateTime GetExtendedRefreshTokenExpiryTime();
    }
} 