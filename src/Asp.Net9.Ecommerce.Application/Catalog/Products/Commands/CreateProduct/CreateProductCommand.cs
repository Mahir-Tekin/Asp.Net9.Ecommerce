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
        public List<ProductVariantInfo> Variants { get; init; } = new();

        // Optional variant type IDs for the product
        public List<Guid>? VariantTypeIds { get; init; }

        // Images for the product
        public List<ProductImageInfo>? Images { get; init; }

        // (Removed) Optional slug for the product
    }

    public record ProductImageInfo
    {
        public string Url { get; init; }
        public string? AltText { get; init; }
        public bool IsMain { get; init; }
    }

    public record ProductVariantInfo
    {
        public string SKU { get; init; }
        public string Name { get; init; }
        public decimal? Price { get; init; }
        public int StockQuantity { get; init; }
        public bool TrackInventory { get; init; }
        // Key: VariationTypeId, Value: VariantOptionId
        public Dictionary<Guid, Guid> SelectedOptions { get; init; } = new();
    }
}