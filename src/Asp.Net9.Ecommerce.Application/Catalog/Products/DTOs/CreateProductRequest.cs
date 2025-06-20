namespace Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs
{
    public class CreateProductRequest
    {
        // Basic product information
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public Guid CategoryId { get; set; }

        // Variants information
        public List<ProductVariantRequest> Variants { get; set; } = new();

        // Optional variant type IDs for the product
        public List<Guid>? VariantTypeIds { get; set; }

        // Images for the product
        public List<ProductImageRequest>? Images { get; set; }
    }

    public class ProductImageRequest
    {
        public string Url { get; set; }
        public string? AltText { get; set; }
        public bool IsMain { get; set; }
    }

    public class ProductVariantRequest
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public int StockQuantity { get; set; }
        public bool TrackInventory { get; set; }
        // Key: VariationTypeId, Value: VariantOptionId
        public Dictionary<Guid, Guid> SelectedOptions { get; set; }
    }

} 