namespace Asp.Net9.Ecommerce.Application.Authentication.DTOs
{
    /// <summary>
    /// Request model for user registration
    /// </summary>
    public record RegisterRequest
    {
        /// <summary>
        /// User's email address (will be used for login)
        /// </summary>
        /// <example>john.doe@example.com</example>
        public string Email { get; init; }

        /// <summary>
        /// Password must be at least 8 characters and contain uppercase, lowercase, number and special character
        /// </summary>
        /// <example>Test123!@#</example>
        public string Password { get; init; }

        /// <summary>
        /// Must match the Password field
        /// </summary>
        /// <example>Test123!@#</example>
        public string ConfirmPassword { get; init; }

        /// <summary>
        /// User's first name
        /// </summary>
        /// <example>John</example>
        public string FirstName { get; init; }

        /// <summary>
        /// User's last name
        /// </summary>
        /// <example>Doe</example>
        public string LastName { get; init; }
    }
} 