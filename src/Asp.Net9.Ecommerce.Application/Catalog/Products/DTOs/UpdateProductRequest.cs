namespace Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs
{
    public class UpdateProductRequest
    {
        // Basic product information
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; }

        // Variants information
        public List<UpdateProductVariantRequest> Variants { get; set; } = new();
    }

    public class UpdateProductVariantRequest
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
        public bool TrackInventory { get; set; }
        public Dictionary<string, string> Variations { get; set; } = new(); // VariationType ID -> Option ID mapping (as strings)
    }
} 