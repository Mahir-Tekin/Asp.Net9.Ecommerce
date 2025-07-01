namespace Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs
{
    public class CreateProductReviewDto
    {
        public Guid ProductId { get; set; }
        public int Rating { get; set; } // 1-5
        public string? Title { get; set; }
        public string? Comment { get; set; }
    }
}
