using Asp.Net9.Ecommerce.Shared.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Asp.Net9.Ecommerce.API.Middleware
{
    public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();
        private readonly JsonSerializerOptions _jsonOptions;

        public CustomAuthorizationMiddlewareResultHandler()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Challenged)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                var error = ErrorResponse.Unauthorized("You must be logged in to access this resource");
                await context.Response.WriteAsJsonAsync(error, _jsonOptions);
                return;
            }
            else if (authorizeResult.Forbidden)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                var error = ErrorResponse.Forbidden("You do not have permission to access this resource");
                await context.Response.WriteAsJsonAsync(error, _jsonOptions);
                return;
            }

            await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
} 