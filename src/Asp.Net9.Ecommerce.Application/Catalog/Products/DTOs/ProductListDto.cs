namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProducts
{
    public class ProductListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? MainImage { get; set; }

        // Variant summary
        public int VariantCount { get; set; }
        public decimal LowestPrice { get; set; }
        public decimal? LowestOldPrice { get; set; }
        public bool HasStock { get; set; }
        public int TotalStock { get; set; }

        // Review statistics
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }

        // Status
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Slug { get; set; }
    }
}