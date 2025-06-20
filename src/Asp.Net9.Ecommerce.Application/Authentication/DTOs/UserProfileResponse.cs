namespace Asp.Net9.Ecommerce.Application.Authentication.DTOs
{
    /// <summary>
    /// Response model for user profile information
    /// </summary>
    public record UserProfileResponse
    {
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
    }
} 