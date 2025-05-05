using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.CreateProduct
{
    public record CreateProductCommand : IRequest<Result<Guid>>
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal BasePrice { get; init; }
        public Guid CategoryId { get; init; }

        // Default variant information
        public string DefaultSKU { get; init; }
        public string DefaultVariantName { get; init; }
        public int? DefaultStockQuantity { get; init; }
        public bool TrackInventory { get; init; }

        // Optional variant types for the product
        public List<VariantTypeInfo>? VariantTypes { get; init; }
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