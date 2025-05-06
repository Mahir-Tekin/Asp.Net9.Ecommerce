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
        public List<ProductVariantRequest> Variants { get; set; }

        // Optional variant types for the product
        public List<VariantTypeRequest>? VariantTypes { get; set; }
    }

    public class ProductVariantRequest
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public int StockQuantity { get; set; }
        public bool TrackInventory { get; set; }
        public Dictionary<string, string> Variations { get; set; }
    }

    public class VariantTypeRequest
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<VariantOptionRequest> Options { get; set; }
    }

    public class VariantOptionRequest
    {
        public string Value { get; set; }
        public string DisplayValue { get; set; }
        public int SortOrder { get; set; }
    }
} 