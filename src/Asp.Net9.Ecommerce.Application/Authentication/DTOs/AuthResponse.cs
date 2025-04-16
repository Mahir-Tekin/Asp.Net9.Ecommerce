namespace Asp.Net9.Ecommerce.Application.Authentication.DTOs
{
    public record AuthResponse
    {
        public bool Succeeded { get; init; }
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
        public string UserId { get; init; }
        public string Email { get; init; }
        public string FullName { get; init; }
        public IList<string> Roles { get; init; }
        public DateTime? ExpiresIn { get; init; }
        public IEnumerable<string> Errors { get; init; }

        private AuthResponse() { }

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