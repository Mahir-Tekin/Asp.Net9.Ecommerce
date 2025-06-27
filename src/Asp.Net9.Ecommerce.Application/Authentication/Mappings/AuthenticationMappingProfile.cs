using AutoMapper;
using Asp.Net9.Ecommerce.Application.Authentication.Commands.Login;
using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using Asp.Net9.Ecommerce.Application.Authentication.Commands.Address;
using Asp.Net9.Ecommerce.Domain.Identity;

namespace Asp.Net9.Ecommerce.Application.Authentication.Mappings
{
    public class AuthenticationMappingProfile : Profile
    {
        public AuthenticationMappingProfile()
        {
            CreateMap<LoginRequest, LoginCommand>();
            CreateMap<CreateAddressRequest, CreateAddressCommand>();
            CreateMap<Address, AddressResponse>();
        }
    }
}