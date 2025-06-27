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

        // Refresh tokens collection
        private readonly List<RefreshToken> _refreshTokens = new();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

        // Address collection
        private readonly List<Address> _addresses = new();
        public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

        // Protected constructor for EF Core
        protected AppUser() { }

        // Factory method for initial registration
        public static Result<AppUser> Create(string email)
        {
            // Domain validation
            if (string.IsNullOrWhiteSpace(email))
                return Result.Failure<AppUser>(ErrorResponse.ValidationError(new List<ValidationError> 
                { 
                    new ValidationError("Email", "Email cannot be empty") 
                }));

            if (!IsValidEmail(email))
                return Result.Failure<AppUser>(ErrorResponse.ValidationError(new List<ValidationError> 
                { 
                    new ValidationError("Email", "Invalid email format") 
                }));

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
            var errors = new List<ValidationError>();

            // Domain validation
            if (firstName?.Length > 50)
                errors.Add(new ValidationError("FirstName", "First name cannot exceed 50 characters"));
            
            if (lastName?.Length > 50)
                errors.Add(new ValidationError("LastName", "Last name cannot exceed 50 characters"));

            if (phoneNumber != null && !IsValidPhoneNumber(phoneNumber))
                errors.Add(new ValidationError("PhoneNumber", "Invalid phone number format"));

            if (errors.Any())
                return Result.Failure(ErrorResponse.ValidationError(errors));

            FirstName = firstName?.Trim();
            LastName = lastName?.Trim();
            PhoneNumber = phoneNumber?.Trim();

            return Result.Success();
        }

        // Refresh token management
        public void AddRefreshToken(string token, DateTime expiresOnUtc)
        {
            _refreshTokens.Add(RefreshToken.Create(token, Id, expiresOnUtc));
        }

        public RefreshToken? GetActiveRefreshToken()
        {
            return _refreshTokens.FirstOrDefault(r => r.IsActive);
        }

        public void RevokeAllRefreshTokens(string reason = "User logged out")
        {
            foreach (var token in _refreshTokens.Where(t => t.IsActive))
            {
                token.Revoke(reason: reason);
            }
        }

        public Result<RefreshToken> ReplaceRefreshToken(string currentToken, string newToken, DateTime expiresOnUtc)
        {
            var existingToken = _refreshTokens.FirstOrDefault(t => t.Token == currentToken);
            
            if (existingToken == null)
                return Result.Failure<RefreshToken>(ErrorResponse.NotFound("Refresh token not found"));

            if (!existingToken.IsActive)
                return Result.Failure<RefreshToken>(ErrorResponse.ValidationError(new List<ValidationError> 
                { 
                    new ValidationError("RefreshToken", "Token is not active") 
                }));

            var newRefreshToken = RefreshToken.Create(newToken, Id, expiresOnUtc);
            _refreshTokens.Add(newRefreshToken);
            
            existingToken.Revoke(newToken, "Replaced by new token");
            
            return Result.Success(newRefreshToken);
        }

        public Result Deactivate()
        {
            if (!IsActive)
                return Result.Failure(ErrorResponse.General("User is already deactivated", "USER_ALREADY_DEACTIVATED"));

            IsActive = false;
            RevokeAllRefreshTokens("User deactivated");
            return Result.Success();
        }

        public Result Activate()
        {
            if (IsActive)
                return Result.Failure(ErrorResponse.General("User is already active", "USER_ALREADY_ACTIVE"));

            IsActive = true;
            return Result.Success();
        }

        // Add address (first address is main, others are not)
        public Result AddAddress(Address address)
        {
            // If this is the first non-deleted address, set as main
            if (!_addresses.Any(a => !a.IsDeleted))
            {
                address.SetAsMain();
            }
            _addresses.Add(address);
            // Ensure only one main address
            if (address.IsMain)
            {
                foreach (var addr in _addresses.Where(a => a.Id != address.Id))
                    addr.UnsetAsMain();
            }
            return Result.Success();
        }

        // Remove address (soft delete)
        public Result RemoveAddress(Guid addressId)
        {
            var address = _addresses.FirstOrDefault(a => a.Id == addressId && !a.IsDeleted);
            if (address == null)
                return Result.Failure(ErrorResponse.NotFound("Address not found"));
            return address.SoftDelete();
        }

        // Set main address
        public Result SetMainAddress(Guid addressId)
        {
            var address = _addresses.FirstOrDefault(a => a.Id == addressId && !a.IsDeleted);
            if (address == null)
                return Result.Failure(ErrorResponse.NotFound("Address not found"));
            foreach (var addr in _addresses)
                addr.UnsetAsMain();
            address.SetAsMain();
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