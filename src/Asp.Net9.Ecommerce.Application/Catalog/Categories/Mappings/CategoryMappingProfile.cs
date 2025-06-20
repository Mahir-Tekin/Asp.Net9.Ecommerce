using Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.CreateCategory;
using Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.UpdateCategory;
using Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs;
using Asp.Net9.Ecommerce.Domain.Catalog;
using AutoMapper;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Mappings
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<CreateCategoryRequest, CreateCategoryCommand>();
            CreateMap<CategoryVariationTypeRequest, CategoryVariationTypeInfo>();
            CreateMap<UpdateCategoryRequest, UpdateCategoryCommand>();

            CreateMap<Category, CategoryNestedDto>()
                .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories))
                .ForMember(dest => dest.VariationTypes, opt => opt.MapFrom(src => src.VariationTypes));

            CreateMap<CategoryVariationType, CategoryVariationTypeDto>();
        }
    }
} 