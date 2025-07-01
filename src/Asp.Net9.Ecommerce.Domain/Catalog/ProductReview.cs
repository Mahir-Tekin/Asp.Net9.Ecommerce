using Asp.Net9.Ecommerce.Domain.Common;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Catalog
{
    public class ProductReview : BaseEntity
    {
        // Basic Information
        public Guid ProductId { get; private set; }
        public Guid UserId { get; private set; }
        public int Rating { get; private set; } // 1-5 stars
        public string? Title { get; private set; }
        public string? Comment { get; private set; }
        
        // Helpfulness tracking
        public int HelpfulVotes { get; private set; }
        public int UnhelpfulVotes { get; private set; }
        
        // Navigation properties
        public Product? Product { get; private set; }
        
        // Vote collection
        private readonly List<ReviewVote> _votes = new();
        public IReadOnlyCollection<ReviewVote> Votes => _votes.AsReadOnly();

        // Protected constructor for EF Core
        protected ProductReview() { }

        // Factory method for creating a new review
        public static Result<ProductReview> Create(
            Guid productId,
            Guid userId,
            int rating,
            string? title = null,
            string? comment = null)
        {
            var errors = new List<ValidationError>();

            // Validate rating
            if (rating < 1 || rating > 5)
                errors.Add(new ValidationError("Rating", "Rating must be between 1 and 5"));

            // Validate title
            if (!string.IsNullOrWhiteSpace(title) && title.Length > 200)
                errors.Add(new ValidationError("Title", "Title cannot exceed 200 characters"));

            // Validate comment
            if (!string.IsNullOrWhiteSpace(comment) && comment.Length > 2000)
                errors.Add(new ValidationError("Comment", "Comment cannot exceed 2000 characters"));

            // At least title or comment should be provided
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(comment))
                errors.Add(new ValidationError("Review", "Either title or comment must be provided"));

            if (errors.Any())
                return Result.Failure<ProductReview>(ErrorResponse.ValidationError(errors));

            var review = new ProductReview
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Title = title?.Trim(),
                Comment = comment?.Trim(),
                HelpfulVotes = 0,
                UnhelpfulVotes = 0
            };

            return Result.Success(review);
        }

        // Update review content
        public Result Update(int rating, string? title = null, string? comment = null)
        {
            var errors = new List<ValidationError>();

            // Validate rating
            if (rating < 1 || rating > 5)
                errors.Add(new ValidationError("Rating", "Rating must be between 1 and 5"));

            // Validate title
            if (!string.IsNullOrWhiteSpace(title) && title.Length > 200)
                errors.Add(new ValidationError("Title", "Title cannot exceed 200 characters"));

            // Validate comment
            if (!string.IsNullOrWhiteSpace(comment) && comment.Length > 2000)
                errors.Add(new ValidationError("Comment", "Comment cannot exceed 2000 characters"));

            // At least title or comment should be provided
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(comment))
                errors.Add(new ValidationError("Review", "Either title or comment must be provided"));

            if (errors.Any())
                return Result.Failure(ErrorResponse.ValidationError(errors));

            Rating = rating;
            Title = title?.Trim();
            Comment = comment?.Trim();
            
            // Update the timestamp
            SetUpdated();

            return Result.Success();
        }

        // Vote management methods
        public Result AddVote(VoteType voteType)
        {
            if (voteType == VoteType.Helpful) 
                HelpfulVotes++;
            else 
                UnhelpfulVotes++;
            
            SetUpdated();
            return Result.Success();
        }

        public Result RemoveVote(VoteType voteType)
        {
            if (voteType == VoteType.Helpful && HelpfulVotes > 0) 
                HelpfulVotes--;
            else if (voteType == VoteType.Unhelpful && UnhelpfulVotes > 0) 
                UnhelpfulVotes--;
            
            SetUpdated();
            return Result.Success();
        }

        public Result ChangeVote(VoteType oldVoteType, VoteType newVoteType)
        {
            // Remove the old vote and add the new one
            RemoveVote(oldVoteType);
            AddVote(newVoteType);
            return Result.Success();
        }

        // Check if user can vote on this review (cannot vote on own review)
        public bool CanUserVote(Guid userId)
        {
            return UserId != userId; // Users cannot vote on their own reviews
        }

        // Get helpfulness ratio
        public double GetHelpfulnessRatio()
        {
            var totalVotes = HelpfulVotes + UnhelpfulVotes;
            return totalVotes == 0 ? 0 : (double)HelpfulVotes / totalVotes;
        }

        // Soft delete review
        public Result Delete()
        {
            SetDeleted();
            return Result.Success();
        }
    }
}