using Asp.Net9.Ecommerce.Shared.Results;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Asp.Net9.Ecommerce.API.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var error = ErrorResponse.General(
                "An unexpected error occurred. Please try again later.",
                "INTERNAL_SERVER_ERROR");

            await context.Response.WriteAsJsonAsync(error, _jsonOptions);
        }
    }
} 