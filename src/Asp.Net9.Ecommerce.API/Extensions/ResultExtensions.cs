using Asp.Net9.Ecommerce.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace Asp.Net9.Ecommerce.API.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result.Value);

            return result.Error.ToActionResult();
        }

        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
                return new OkResult();

            return result.Error.ToActionResult();
        }
    }
} 