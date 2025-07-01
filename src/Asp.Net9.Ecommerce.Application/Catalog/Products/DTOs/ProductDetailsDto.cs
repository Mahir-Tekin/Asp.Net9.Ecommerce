using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs
{
    public class ProductDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        /// <summary>
        /// The URL of the main product image.
        /// </summary>
        public string? MainImage { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
        public string? CategoryName { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<VariationTypeDto> VariationTypes { get; set; } = new();
        public List<ProductVariantDetailsDto> Variants { get; set; } = new();
        public int TotalStock { get; set; }
        public bool HasStock { get; set; }
        public decimal LowestPrice { get; set; }
        public decimal? LowestOldPrice { get; set; }
        
        // Review Statistics
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class ProductImageDto
    {
        public string Url { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public bool IsMain { get; set; }
    }

    public class ProductVariantDetailsDto
    {
        public Guid Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int StockQuantity { get; set; }
        public bool TrackInventory { get; set; }
        public bool IsActive { get; set; }
        public Dictionary<Guid, Guid> SelectedOptions { get; set; } = new(); // Key: VariationTypeId, Value: VariantOptionId
    }
}
