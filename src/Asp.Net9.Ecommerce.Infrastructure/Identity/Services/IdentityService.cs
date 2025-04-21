using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Infrastructure.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public IdentityService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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

        public async Task<Result> UpdateRefreshTokenAsync(string userId, string refreshToken, DateTime refreshTokenExpiryTime)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure(ErrorResponse.NotFound("User not found"));

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiryTime;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(e => new ValidationError(e.Code, e.Description))
                    .ToList();
                return Result.Failure(ErrorResponse.ValidationError(errors));
            }

            return Result.Success();
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