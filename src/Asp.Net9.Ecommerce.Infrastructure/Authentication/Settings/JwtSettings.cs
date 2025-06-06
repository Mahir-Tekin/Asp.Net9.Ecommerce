namespace Asp.Net9.Ecommerce.Infrastructure.Authentication.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; init; }
        public string Issuer { get; init; }
        public string Audience { get; init; }
        public int ExpirationInMinutes { get; init; }
        public int RefreshTokenExpirationInDays { get; init; }
        public int ExtendedRefreshTokenExpirationInDays { get; init; } = 30; // Default 30 days for "Remember Me"
    }
} 