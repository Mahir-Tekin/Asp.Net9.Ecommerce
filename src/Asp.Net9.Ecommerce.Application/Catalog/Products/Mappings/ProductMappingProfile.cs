using AutoMapper;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.CreateProduct;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.UpdateProduct;
using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProductById;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProducts;
using Asp.Net9.Ecommerce.Domain.Catalog;
using System.Linq;
using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Mappings
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            // --- CREATE PRODUCT MAPPINGS ---
            CreateMap<CreateProductRequest, CreateProductCommand>();
            CreateMap<ProductVariantRequest, ProductVariantInfo>();
            CreateMap<ProductImageRequest, ProductImageInfo>();

            // --- COMMENTED OUT: Needs refactor for new structure ---
            //CreateMap<VariantTypeRequest, VariantTypeInfo>();
            //CreateMap<VariantOptionRequest, VariantOptionInfo>();
            //CreateMap<UpdateProductRequest, UpdateProductCommand>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore());
            //CreateMap<UpdateProductVariantRequest, ProductVariantUpdateInfo>();
            //CreateMap<Product, ProductDetailsDto>()
            //    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            //    .ForMember(dest => dest.MainImage, opt => opt.MapFrom(src => src.MainImage));
            //CreateMap<ProductVariant, ProductVariantDetailsDto>()
            //    .ForMember(dest => dest.Variations, opt => opt.MapFrom(src => src.GetVariations()));
            CreateMap<Product, ProductListDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.MainImage, opt => opt.MapFrom(src => src.MainImage))
                .ForMember(dest => dest.VariantCount, opt => opt.MapFrom(src => src.Variants.Count))
                .ForMember(dest => dest.LowestPrice, opt => opt.MapFrom(src => src.Variants.Any() ? src.Variants.Min(v => v.Price) : src.BasePrice))
                .ForMember(dest => dest.LowestOldPrice, opt => opt.MapFrom(src =>
                    src.Variants.Any()
                        ? src.Variants.OrderBy(v => v.Price).FirstOrDefault().OldPrice
                        : (decimal?)null))
                .ForMember(dest => dest.HasStock, opt => opt.MapFrom(src => src.Variants.Any(v => v.TrackInventory ? v.StockQuantity > 0 : true)))
                .ForMember(dest => dest.TotalStock, opt => opt.MapFrom(src => src.Variants.Where(v => v.TrackInventory).Sum(v => v.StockQuantity)))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug));

            // --- PRODUCT DETAILS MAPPINGS ---
            CreateMap<Product, ProductDetailsDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ForMember(dest => dest.VariationTypes, opt => opt.MapFrom(src => src.VariantTypes))
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants))
                .ForMember(dest => dest.MainImage, opt => opt.MapFrom(src => src.MainImage))
                .ForMember(dest => dest.TotalStock, opt => opt.MapFrom(src => src.Variants.Where(v => v.TrackInventory).Sum(v => v.StockQuantity)))
                .ForMember(dest => dest.HasStock, opt => opt.MapFrom(src => src.Variants.Any(v => v.TrackInventory ? v.StockQuantity > 0 : true)))
                .ForMember(dest => dest.LowestPrice, opt => opt.MapFrom(src => src.Variants.Any() ? src.Variants.Min(v => v.Price) : src.BasePrice))
                .ForMember(dest => dest.LowestOldPrice, opt => opt.MapFrom(src => src.Variants.Any() ? src.Variants.OrderBy(v => v.Price).FirstOrDefault().OldPrice : (decimal?)null));

            CreateMap<ProductImage, ProductImageDto>();

            CreateMap<VariationType, VariationTypeDto>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));

            CreateMap<VariantOption, VariantOptionDto>();

            CreateMap<ProductVariant, ProductVariantDetailsDto>()
                .ForMember(dest => dest.SelectedOptions, opt => opt.MapFrom(src => src.SelectedOptions.ToDictionary(
                    o => o.VariationTypeId,
                    o => o.Id
                )));
        }
    }
}