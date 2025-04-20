using Asp.Net9.Ecommerce.Domain.Common;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Identity
{
    public class AppUser : IdentityUser<Guid>
    {
        // Personal Information - all nullable since they're optional
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        
        // Computed Property - handle null cases
        public string? FullName => (FirstName != null || LastName != null) 
            ? $"{FirstName} {LastName}".Trim() 
            : null;
        
        public bool IsActive { get; private set; } = true;

        // Token management
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Protected constructor for EF Core
        protected AppUser() { }

        // Factory method for initial registration
        public static Result<AppUser> Create(string email)
        {
            // Domain validation
            if (string.IsNullOrWhiteSpace(email))
                return Result.Failure<AppUser>("Email cannot be empty");

            if (!IsValidEmail(email))
                return Result.Failure<AppUser>("Invalid email format");

            var user = new AppUser
            {
                Email = email.Trim().ToLower(),
                UserName = email.Trim().ToLower(),
                IsActive = true
            };

            return Result.Success(user);
        }

        // Method to update profile information
        public Result UpdateProfile(string? firstName, string? lastName, string? phoneNumber)
        {
            // Domain validation
            if (firstName?.Length > 50)
                return Result.Failure("First name cannot exceed 50 characters");
            
            if (lastName?.Length > 50)
                return Result.Failure("Last name cannot exceed 50 characters");

            if (phoneNumber != null && !IsValidPhoneNumber(phoneNumber))
                return Result.Failure("Invalid phone number format");

            FirstName = firstName?.Trim();
            LastName = lastName?.Trim();
            PhoneNumber = phoneNumber?.Trim();

            return Result.Success();
        }

        public Result Deactivate()
        {
            if (!IsActive)
                return Result.Failure("User is already deactivated");

            IsActive = false;
            return Result.Success();
        }

        public Result Activate()
        {
            if (IsActive)
                return Result.Failure("User is already active");

            IsActive = true;
            return Result.Success();
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            // Simple validation - can be enhanced based on requirements
            return Regex.IsMatch(phoneNumber, @"^\+?[\d\s-]{8,}$");
        }
    }
} 