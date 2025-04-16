using Asp.Net9.Ecommerce.Domain.Identity;

namespace Asp.Net9.Ecommerce.Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(string userId, IEnumerable<string> roles);
        string GenerateRefreshToken();
        DateTime GetRefreshTokenExpiryTime();
    }
} 