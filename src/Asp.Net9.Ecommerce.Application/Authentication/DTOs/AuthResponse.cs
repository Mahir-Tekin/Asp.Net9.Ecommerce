namespace Asp.Net9.Ecommerce.Application.Authentication.DTOs
{
    /// <summary>
    /// Response model for authentication operations (login/register)
    /// </summary>
    public record AuthResponse
    {
        /// <summary>
        /// Indicates if the authentication operation was successful
        /// </summary>
        public bool Succeeded { get; init; }

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
        public DateTime? ExpiresIn { get; init; }

        /// <summary>
        /// List of error messages if the operation failed
        /// </summary>
        public IEnumerable<string> Errors { get; init; }

        private AuthResponse() { }

        /// <summary>
        /// Creates a successful authentication response
        /// </summary>
        public static AuthResponse Success(
            string accessToken,
            string refreshToken,
            string userId,
            string email,
            string fullName,
            IList<string> roles,
            DateTime expiresIn) =>
            new()
            {
                Succeeded = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = userId,
                Email = email,
                FullName = fullName,
                Roles = roles,
                ExpiresIn = expiresIn,
                Errors = Array.Empty<string>()
            };

        /// <summary>
        /// Creates a failed authentication response with error messages
        /// </summary>
        public static AuthResponse Failure(IEnumerable<string> errors) =>
            new()
            {
                Succeeded = false,
                AccessToken = string.Empty,
                RefreshToken = string.Empty,
                UserId = string.Empty,
                Email = string.Empty,
                FullName = string.Empty,
                Roles = Array.Empty<string>(),
                ExpiresIn = null,
                Errors = errors
            };
    }
} 