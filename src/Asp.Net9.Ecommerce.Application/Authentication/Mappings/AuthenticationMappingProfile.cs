using AutoMapper;
using Asp.Net9.Ecommerce.Application.Authentication.Commands.Login;
using Asp.Net9.Ecommerce.Application.Authentication.DTOs;

namespace Asp.Net9.Ecommerce.Application.Authentication.Mappings
{
    public class AuthenticationMappingProfile : Profile
    {
        public AuthenticationMappingProfile()
        {
            CreateMap<LoginRequest, LoginCommand>();
        }
    }
} 