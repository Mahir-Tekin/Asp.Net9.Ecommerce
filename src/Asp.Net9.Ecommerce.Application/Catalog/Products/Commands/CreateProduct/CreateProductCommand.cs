using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.CreateProduct
{
    public record CreateProductCommand : IRequest<Result<Guid>>
    {
        // Basic product information
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal BasePrice { get; init; }
        public Guid CategoryId { get; init; }

        // Variants information
        public List<ProductVariantInfo> Variants { get; init; }

        // Optional variant types for the product
        public List<VariantTypeInfo>? VariantTypes { get; init; }
    }

    public record ProductVariantInfo
    {
        public string SKU { get; init; }
        public string Name { get; init; }
        public decimal? Price { get; init; }
        public int StockQuantity { get; init; }
        public bool TrackInventory { get; init; }
        public Dictionary<string, string> Variations { get; init; }
    }

    public record VariantTypeInfo
    {
        public string Name { get; init; }
        public string DisplayName { get; init; }
        public List<VariantOptionInfo> Options { get; init; }
    }

    public record VariantOptionInfo
    {
        public string Value { get; init; }
        public string DisplayValue { get; init; }
        public int SortOrder { get; init; }
    }
} 