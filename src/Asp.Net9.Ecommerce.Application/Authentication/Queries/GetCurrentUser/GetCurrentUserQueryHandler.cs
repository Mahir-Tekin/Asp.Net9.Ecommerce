using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces.ServiceInterfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Authentication.Queries.GetCurrentUser
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserProfileResponse>>
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtService _jwtService;

        public GetCurrentUserQueryHandler(
            IIdentityService identityService,
            IJwtService jwtService)
        {
            _identityService = identityService;
            _jwtService = jwtService;
        }

        public async Task<Result<UserProfileResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            // 1. Get user roles
            var rolesResult = await _identityService.GetUserRolesAsync(request.UserId);
            if (rolesResult.IsFailure)
                return Result.Failure<UserProfileResponse>(rolesResult.Error);

            // 2. Get user details
            var userDetailsResult = await _identityService.GetUserDetailsAsync(request.UserId);
            if (userDetailsResult.IsFailure)
                return Result.Failure<UserProfileResponse>(userDetailsResult.Error);

            var (firstName, lastName, email) = userDetailsResult.Value;

            // 3. Return user information
            var response = new UserProfileResponse
            {
                UserId = request.UserId,
                Email = email,
                FullName = $"{firstName} {lastName}".Trim(),
                Roles = rolesResult.Value.ToList()
            };

            return Result.Success(response);
        }
    }
} 