using Asp.Net9.Ecommerce.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Asp.Net9.Ecommerce.Infrastructure.Persistence
{
    public class ApplicationDbInitializer
    {
        private readonly ILogger<ApplicationDbInitializer> _logger;
        private readonly UserManager<AppUser> _userManager;

        public ApplicationDbInitializer(
            ILogger<ApplicationDbInitializer> logger,
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task InitializeAsync()
        {
            try
            {
                await SeedAdminUserAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the admin user.");
                throw;
            }
        }

        private async Task SeedAdminUserAsync()
        {
            // Use environment variables for admin credentials; do not seed if not set
            var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL");
            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                _logger.LogInformation("ADMIN_EMAIL or ADMIN_PASSWORD not set. Skipping admin user seeding.");
                return;
            }

            if (_userManager.Users.All(u => u.UserName != adminEmail))
            {
                _logger.LogInformation("Seeding admin user...");

                var userResult = AppUser.Create(adminEmail);
                if (userResult.IsFailure)
                {
                    _logger.LogError("Failed to create admin user: {Error}", userResult.Error);
                    return;
                }

                var administrator = userResult.Value;
                var profileResult = administrator.UpdateProfile("System", "Admin", null);
                if (profileResult.IsFailure)
                {
                    _logger.LogError("Failed to update admin profile: {Error}", profileResult.Error);
                    return;
                }

                var result = await _userManager.CreateAsync(administrator, adminPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRolesAsync(administrator, new[] { AppRoles.Admin });
                    _logger.LogInformation("Admin user created successfully");
                }
                else
                {
                    _logger.LogError("Failed to create admin user. Errors: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
} 