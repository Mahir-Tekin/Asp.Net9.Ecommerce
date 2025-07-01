using Asp.Net9.Ecommerce.Domain.Common;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Catalog
{
    public class ReviewVote : BaseEntity
    {
        // Basic Information
        public Guid ReviewId { get; private set; }
        public Guid UserId { get; private set; }
        public VoteType VoteType { get; private set; }
        
        // Navigation properties
        public ProductReview? Review { get; private set; }

        // Protected constructor for EF Core
        protected ReviewVote() { }

        // Factory method for creating a new vote
        public static Result<ReviewVote> Create(Guid reviewId, Guid userId, VoteType voteType)
        {
            var vote = new ReviewVote
            {
                ReviewId = reviewId,
                UserId = userId,
                VoteType = voteType
            };

            return Result.Success(vote);
        }

        // Update vote type (for changing from helpful to unhelpful or vice versa)
        public Result UpdateVoteType(VoteType newVoteType)
        {
            VoteType = newVoteType;
            SetUpdated();
            return Result.Success();
        }
    }
}
