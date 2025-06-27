using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces.ServiceInterfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Authentication.Queries.Address
{
    public class GetAddressesByUserIdQueryHandler : IRequestHandler<GetAddressesByUserIdQuery, Result<List<AddressResponse>>>
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public GetAddressesByUserIdQueryHandler(IIdentityService identityService, IMapper mapper)
        {
            _identityService = identityService;
            _mapper = mapper;
        }

        public async Task<Result<List<AddressResponse>>> Handle(GetAddressesByUserIdQuery request, CancellationToken cancellationToken)
        {
            var addresses = await _identityService.GetAddressesByUserIdAsync(request.UserId, cancellationToken);
            var response = _mapper.Map<List<AddressResponse>>(addresses);
            return Result.Success(response);
        }
    }
}
