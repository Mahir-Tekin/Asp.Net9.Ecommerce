using Asp.Net9.Ecommerce.Domain.Identity;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        /// <summary>
        /// Validates user credentials and returns user information if valid
        /// </summary>
        Task<Result<(string userId, AppUser user)>> ValidateCredentialsAsync(string email, string password);

        /// <summary>
        /// Checks if a user account is locked out
        /// </summary>
        Task<Result<bool>> IsLockedOutAsync(string userId);

        /// <summary>
        /// Gets the roles assigned to a user
        /// </summary>
        Task<Result<IList<string>>> GetUserRolesAsync(string userId);

        /// <summary>
        /// Creates a new user account
        /// </summary>
        Task<Result<string>> CreateUserAsync(string email, string password, string firstName, string lastName);

        /// <summary>
        /// Finds a user by their email address
        /// </summary>
        Task<Result<AppUser>> FindByEmailAsync(string email);

        /// <summary>
        /// Updates the refresh token for a user
        /// </summary>
        Task<Result> UpdateRefreshTokenAsync(string userId, string refreshToken, DateTime refreshTokenExpiryTime);

        /// <summary>
        /// Gets basic user details
        /// </summary>
        Task<Result<(string firstName, string lastName, string email)>> GetUserDetailsAsync(string userId);

        /// <summary>
        /// Assigns a role to a user
        /// </summary>
        Task<Result> AssignUserToRoleAsync(string userId, string role);
    }
} 