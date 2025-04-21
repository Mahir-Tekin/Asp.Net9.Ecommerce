namespace Asp.Net9.Ecommerce.Application.Authentication.DTOs
{
    /// <summary>
    /// Response model for authentication operations (login/register)
    /// </summary>
    public record AuthResponse
    {
        /// <summary>
        /// JWT access token for API authentication
        /// </summary>
        public string AccessToken { get; init; }

        /// <summary>
        /// Refresh token for obtaining new access tokens
        /// </summary>
        public string RefreshToken { get; init; }

        /// <summary>
        /// Unique identifier of the user
        /// </summary>
        public string UserId { get; init; }

        /// <summary>
        /// User's email address
        /// </summary>
        public string Email { get; init; }

        /// <summary>
        /// User's full name (FirstName + LastName)
        /// </summary>
        public string FullName { get; init; }

        /// <summary>
        /// List of roles assigned to the user
        /// </summary>
        public IList<string> Roles { get; init; }

        /// <summary>
        /// Expiration time of the refresh token
        /// </summary>
        public DateTime RefreshTokenExpiryTime { get; init; }
    }
} 