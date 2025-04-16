using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Domain.Identity;
using Asp.Net9.Ecommerce.Infrastructure.Authentication.Services;
using Asp.Net9.Ecommerce.Infrastructure.Authentication.Settings;
using Asp.Net9.Ecommerce.Infrastructure.Identity;
using Asp.Net9.Ecommerce.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Asp.Net9.Ecommerce.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add Identity Services (including DbContext)
            services.AddIdentityServices(configuration);
            
            // Add JWT Settings
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            
            // Add JWT Service
            services.AddScoped<IJwtService, JwtService>();

            // Add Identity Service
            services.AddScoped<IIdentityService, IdentityService>();

            // Add JWT Authentication
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings?.Issuer,
                    ValidAudience = jwtSettings?.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? throw new InvalidOperationException("JWT SecretKey is not configured")))
                };
            });

            return services;
        }
    }
} 