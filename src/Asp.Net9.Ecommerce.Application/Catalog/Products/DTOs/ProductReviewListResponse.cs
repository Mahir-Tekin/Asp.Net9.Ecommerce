namespace Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs
{
    public class ProductReviewListResponse
    {
        public List<ProductReviewDto> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        
        // Additional review-specific metadata
        public Guid ProductId { get; set; }
        public double AverageRating { get; set; }
        public ReviewRatingSummary RatingSummary { get; set; } = new();
    }
    
    public class ReviewRatingSummary
    {
        public int FiveStars { get; set; }
        public int FourStars { get; set; }
        public int ThreeStars { get; set; }
        public int TwoStars { get; set; }
        public int OneStar { get; set; }
        public int TotalReviews => FiveStars + FourStars + ThreeStars + TwoStars + OneStar;
        
        // Percentage calculations
        public double FiveStarPercentage => TotalReviews > 0 ? (double)FiveStars / TotalReviews * 100 : 0;
        public double FourStarPercentage => TotalReviews > 0 ? (double)FourStars / TotalReviews * 100 : 0;
        public double ThreeStarPercentage => TotalReviews > 0 ? (double)ThreeStars / TotalReviews * 100 : 0;
        public double TwoStarPercentage => TotalReviews > 0 ? (double)TwoStars / TotalReviews * 100 : 0;
        public double OneStarPercentage => TotalReviews > 0 ? (double)OneStar / TotalReviews * 100 : 0;
    }
}
