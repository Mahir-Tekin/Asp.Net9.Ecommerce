using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProductById
{
    public record GetProductByIdQuery : IRequest<Result<ProductDetailsDto>>
    {
        public Guid Id { get; init; }
    }

    public class ProductDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? MainImage { get; set; }

        // Category info
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }

        // Variants
        public List<ProductVariantDetailsDto> Variants { get; set; }
    }

    public class ProductVariantDetailsDto
    {
        public Guid Id { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool TrackInventory { get; set; }
        public bool IsActive { get; set; }
        public Dictionary<string, string> Variations { get; set; }
    }
} 