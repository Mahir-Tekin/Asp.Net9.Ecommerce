namespace Asp.Net9.Ecommerce.Application.Authentication.DTOs
{
    public record RegisterRequest
    {
        public string Email { get; init; }
        public string Password { get; init; }
        public string ConfirmPassword { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
    }
} 