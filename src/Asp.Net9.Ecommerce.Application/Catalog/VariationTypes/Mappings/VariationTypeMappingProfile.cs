using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Commands.CreateVariationType;
using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Commands.UpdateVariationType;
using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs;
using Asp.Net9.Ecommerce.Domain.Catalog;
using AutoMapper;

namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Mappings
{
    public class VariationTypeMappingProfile : Profile
    {
        public VariationTypeMappingProfile()
        {
            CreateMap<CreateVariationTypeRequest, CreateVariationTypeCommand>();
            CreateMap<CreateVariationOptionRequest, VariationOptionInfo>();

            // Update mappings
            CreateMap<UpdateVariationTypeRequest, UpdateVariationTypeCommand>();
            CreateMap<UpdateVariationOptionRequest, VariationOptionUpdateInfo>();

            // Response mappings
            CreateMap<VariationType, VariationTypeResponse>();
            CreateMap<VariantOption, VariationOptionResponse>();
        }
    }
} 