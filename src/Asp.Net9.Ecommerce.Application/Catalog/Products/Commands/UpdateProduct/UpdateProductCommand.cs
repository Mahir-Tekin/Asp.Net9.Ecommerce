using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand : IRequest<Result>
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public string? Description { get; init; }
        public decimal BasePrice { get; init; }
        public bool IsActive { get; init; }
        public List<ProductVariantUpdateInfo> Variants { get; init; } = new();
    }

    public record ProductVariantUpdateInfo
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public decimal? Price { get; init; }
        public int? StockQuantity { get; init; }
        public bool TrackInventory { get; init; }
        public Dictionary<Guid, Guid> SelectedOptions { get; init; } = new(); // VariationType ID -> Option ID mapping
    }
}
