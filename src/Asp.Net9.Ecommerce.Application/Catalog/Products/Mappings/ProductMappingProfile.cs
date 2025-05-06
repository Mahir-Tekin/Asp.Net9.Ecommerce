using AutoMapper;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.CreateProduct;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.UpdateProduct;
using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProductById;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProducts;
using Asp.Net9.Ecommerce.Domain.Catalog;
using System.Linq;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Mappings
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<CreateProductRequest, CreateProductCommand>();
            CreateMap<ProductVariantRequest, ProductVariantInfo>();
            CreateMap<VariantTypeRequest, VariantTypeInfo>();
            CreateMap<VariantOptionRequest, VariantOptionInfo>();

            CreateMap<UpdateProductRequest, UpdateProductCommand>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<UpdateProductVariantRequest, ProductVariantUpdateInfo>();

            CreateMap<Product, ProductDetailsDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.MainImage, opt => opt.MapFrom(src => src.MainImage));

            CreateMap<ProductVariant, ProductVariantDetailsDto>()
                .ForMember(dest => dest.Variations, opt => opt.MapFrom(src => src.GetVariations()));

            CreateMap<Product, ProductListDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.MainImage, opt => opt.MapFrom(src => src.MainImage))
                .ForMember(dest => dest.VariantCount, opt => opt.MapFrom(src => src.Variants.Count))
                .ForMember(dest => dest.LowestPrice, opt => opt.MapFrom(src => 
                    src.Variants.Any() ? src.Variants.Min(v => v.Price) : src.BasePrice))
                .ForMember(dest => dest.HighestPrice, opt => opt.MapFrom(src => 
                    src.Variants.Any() ? src.Variants.Max(v => v.Price) : src.BasePrice))
                .ForMember(dest => dest.HasStock, opt => opt.MapFrom(src => 
                    src.Variants.Any(v => v.TrackInventory ? v.StockQuantity > 0 : true)))
                .ForMember(dest => dest.TotalStock, opt => opt.MapFrom(src => 
                    src.Variants.Where(v => v.TrackInventory).Sum(v => v.StockQuantity)));
        }
    }
} 