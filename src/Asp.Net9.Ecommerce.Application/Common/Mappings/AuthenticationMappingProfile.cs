using Asp.Net9.Ecommerce.Application.Authentication.Commands.Register;
using Asp.Net9.Ecommerce.Application.Authentication.DTOs;
using AutoMapper;

namespace Asp.Net9.Ecommerce.Application.Common.Mappings
{
    public class AuthenticationMappingProfile : Profile
    {
        public AuthenticationMappingProfile()
        {
            CreateMap<RegisterRequest, RegisterCommand>();
            
            // If we need custom mapping:
            /*
            CreateMap<RegisterRequest, RegisterCommand>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower()));
            */
        }
    }
} 