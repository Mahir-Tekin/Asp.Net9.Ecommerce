using Asp.Net9.Ecommerce.Shared.Results;
using Asp.Net9.Ecommerce.Application.Common.Interfaces.ServiceInterfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asp.Net9.Ecommerce.Application.Authentication.Commands.Address
{
    public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, Result<Guid>>
    {
        private readonly IIdentityService _identityService;

        public CreateAddressCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<Guid>> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.AddAddressAsync(
                userId: request.UserId,
                firstName: request.FirstName,
                lastName: request.LastName,
                phoneNumber: request.PhoneNumber,
                city: request.City,
                district: request.District,
                neighborhood: request.Neighborhood,
                addressLine: request.AddressLine,
                addressTitle: request.AddressTitle,
                cancellationToken: cancellationToken
            );
        }
    }
}
