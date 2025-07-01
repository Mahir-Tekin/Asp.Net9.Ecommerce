namespace Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs
{
    public class ProductReviewDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; } // 1-5 stars
        public string? Title { get; set; }
        public string? Comment { get; set; }
        
        // Helpfulness tracking
        public int HelpfulVotes { get; set; }
        public int UnhelpfulVotes { get; set; }
        public int TotalHelpfulnessVotes => HelpfulVotes + UnhelpfulVotes;
        public double HelpfulnessRatio => TotalHelpfulnessVotes > 0 ? (double)HelpfulVotes / TotalHelpfulnessVotes : 0;
        
        // User information (censored for privacy)
        public string ReviewerName { get; set; } = string.Empty; // e.g., "Mahir T****"
        public bool IsVerifiedPurchase { get; set; } = true; // Since we only allow reviews from purchasers
        
        // Timestamps
        public DateTime CreatedAt { get; set; }
    }
}
