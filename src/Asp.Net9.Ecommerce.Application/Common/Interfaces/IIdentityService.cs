using Asp.Net9.Ecommerce.Domain.Identity;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<Result<(string userId, AppUser user)>> ValidateCredentialsAsync(string email, string password);
        Task<bool> IsLockedOutAsync(string userId);
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<Result<string>> CreateUserAsync(string email, string password, string firstName, string lastName);
        Task<Result<AppUser>> FindByEmailAsync(string email);
        Task<Result> UpdateRefreshTokenAsync(string userId, string refreshToken, DateTime refreshTokenExpiryTime);
        Task<(string firstName, string lastName, string email)> GetUserDetailsAsync(string userId);
    }
} 