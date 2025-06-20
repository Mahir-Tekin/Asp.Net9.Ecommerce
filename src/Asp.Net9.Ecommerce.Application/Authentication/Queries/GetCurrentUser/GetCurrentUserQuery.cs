using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Authentication.Queries.GetCurrentUser
{
    /// <summary>
    /// Query to get current user's information
    /// </summary>
    public record GetCurrentUserQuery(string UserId) : IRequest<Result<UserProfileResponse>>;
} 