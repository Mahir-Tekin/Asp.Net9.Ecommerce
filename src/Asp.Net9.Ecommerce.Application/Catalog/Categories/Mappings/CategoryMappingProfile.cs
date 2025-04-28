using Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.CreateCategory;
using Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs;
using AutoMapper;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Mappings
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<CreateCategoryRequest, CreateCategoryCommand>();
        }
    }
} 