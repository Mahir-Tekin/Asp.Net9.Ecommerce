using Asp.Net9.Ecommerce.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace Asp.Net9.Ecommerce.API.Extensions
{
    public static class ErrorResponseExtensions
    {
        public static IActionResult ToActionResult(this ErrorResponse error)
        {
            var statusCode = error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            return new ObjectResult(error) { StatusCode = statusCode };
        }
    }
} 