using Asp.Net9.Ecommerce.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Net9.Ecommerce.Shared.Results;
using Microsoft.Extensions.Logging;
using Asp.Net9.Ecommerce.Application.Common.Interfaces.ServiceInterfaces;

namespace Asp.Net9.Ecommerce.Infrastructure.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppIdentityDbContext _context;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            AppIdentityDbContext context,
            ILogger<IdentityService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        public async Task<Result<(string userId, AppUser user)>> ValidateCredentialsAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result.Failure<(string userId, AppUser user)>(
                    ErrorResponse.ValidationError(new List<ValidationError> 
                    { 
                        new ValidationError("Credentials", "Invalid email or password") 
                    }));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    return Result.Failure<(string userId, AppUser user)>(
                        ErrorResponse.General("Account is locked out", "ACCOUNT_LOCKED"));
                
                if (result.IsNotAllowed)
                    return Result.Failure<(string userId, AppUser user)>(
                        ErrorResponse.General("Login is not allowed", "LOGIN_NOT_ALLOWED"));
                
                return Result.Failure<(string userId, AppUser user)>(
                    ErrorResponse.ValidationError(new List<ValidationError> 
                    { 
                        new ValidationError("Credentials", "Invalid email or password") 
                    }));
            }

            return Result.Success((user.Id.ToString(), user));
        }

        public async Task<Result<bool>> IsLockedOutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure<bool>(ErrorResponse.NotFound("User not found"));
            }
            return Result.Success(await _userManager.IsLockedOutAsync(user));
        }

        public async Task<Result<IList<string>>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure<IList<string>>(ErrorResponse.NotFound("User not found"));
            }
            return Result.Success(await _userManager.GetRolesAsync(user));
        }

        public async Task<Result<string>> CreateUserAsync(string email, string password, string firstName, string lastName)
        {
            // Create user using factory method
            var userResult = AppUser.Create(email);
            if (userResult.IsFailure)
                return Result.Failure<string>(userResult.Error);

            var user = userResult.Value;
            // Update profile with first and last name
            var profileUpdateResult = user.UpdateProfile(firstName, lastName, null);
            if (profileUpdateResult.IsFailure)
                return Result.Failure<string>(profileUpdateResult.Error);

            var createUserResult = await _userManager.CreateAsync(user, password);
            if (!createUserResult.Succeeded)
            {
                var errors = createUserResult.Errors
                    .Select(e => new ValidationError(e.Code, e.Description))
                    .ToList();
                return Result.Failure<string>(ErrorResponse.ValidationError(errors));
            }

            return Result.Success(user.Id.ToString());
        }

        public async Task<Result<AppUser>> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result.Failure<AppUser>(ErrorResponse.NotFound("User not found"));
            }
            return Result.Success(user);
        }

        public async Task<Result<RefreshToken>> AddRefreshTokenAsync(string userId, string token, DateTime expiryTime)
        {
            _logger.LogDebug("Starting AddRefreshTokenAsync for User {UserId}", userId);
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure<RefreshToken>(ErrorResponse.NotFound("User not found"));

            _logger.LogDebug("Before AddRefreshToken - User {UserId} ConcurrencyStamp: {ConcurrencyStamp}", 
                userId, user.ConcurrencyStamp);

            // Adding delay to simulate slower processing and help identify parallel operations
            _logger.LogDebug("Sleeping for 5 seconds to simulate slow processing...");
            await Task.Delay(5000); // 5 second delay

            var beforeUpdateStamp = user.ConcurrencyStamp;
            user.AddRefreshToken(token, expiryTime);
            _logger.LogDebug("After AddRefreshToken, before UpdateAsync - ConcurrencyStamp: {ConcurrencyStamp}", user.ConcurrencyStamp);

            // Check if user has been modified in database
            var currentUserInDb = await _userManager.FindByIdAsync(userId);
            _logger.LogDebug("Current user in database - ConcurrencyStamp: {ConcurrencyStamp}", 
                currentUserInDb?.ConcurrencyStamp);

            var result = await _userManager.UpdateAsync(user);

            _logger.LogDebug("After UpdateAsync - User {UserId} Original ConcurrencyStamp: {OriginalStamp}, Current ConcurrencyStamp: {CurrentStamp}, Success: {Succeeded}", 
                userId, beforeUpdateStamp, user.ConcurrencyStamp, result.Succeeded);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(e => new ValidationError(e.Code, e.Description))
                    .ToList();
                _logger.LogWarning("Failed to update user {UserId}. Errors: {@Errors}", userId, errors);
                return Result.Failure<RefreshToken>(ErrorResponse.ValidationError(errors));
            }

            return Result.Success(user.GetActiveRefreshToken());
        }

        public async Task<Result<RefreshToken>> GetActiveRefreshTokenAsync(string userId, string token)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

            if (user == null)
                return Result.Failure<RefreshToken>(ErrorResponse.NotFound("User not found"));

            var refreshToken = user.RefreshTokens.FirstOrDefault(t => t.Token == token && t.IsActive);
            
            if (refreshToken == null)
                return Result.Failure<RefreshToken>(ErrorResponse.NotFound("Active refresh token not found"));

            return Result.Success(refreshToken);
        }

        public async Task<Result> RevokeAllRefreshTokensAsync(string userId, string reason = "User logged out")
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

            if (user == null)
                return Result.Failure(ErrorResponse.NotFound("User not found"));

            user.RevokeAllRefreshTokens(reason);
            await _context.SaveChangesAsync();
            
            return Result.Success();
        }

        public async Task<Result<RefreshToken>> ReplaceRefreshTokenAsync(string userId, string currentToken, string newToken, DateTime expiryTime)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

            if (user == null)
                return Result.Failure<RefreshToken>(ErrorResponse.NotFound("User not found"));

            var result = user.ReplaceRefreshToken(currentToken, newToken, expiryTime);
            if (result.IsFailure)
                return Result.Failure<RefreshToken>(result.Error);

            await _context.SaveChangesAsync();
            return Result.Success(result.Value);
        }

        public async Task<Result<(string firstName, string lastName, string email)>> GetUserDetailsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure<(string firstName, string lastName, string email)>(
                    ErrorResponse.NotFound("User not found"));
            }
            return Result.Success((user.FirstName ?? string.Empty, user.LastName ?? string.Empty, user.Email));
        }

        public async Task<Result> AssignUserToRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure(ErrorResponse.NotFound("User not found"));

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(e => new ValidationError(e.Code, e.Description))
                    .ToList();
                return Result.Failure(ErrorResponse.ValidationError(errors));
            }

            return Result.Success();
        }
    }
} 