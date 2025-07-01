using Asp.Net9.Ecommerce.Domain.Catalog;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.VoteOnProductReview
{
    public record VoteOnProductReviewCommand : IRequest<Result>
    {
        public Guid ProductId { get; init; }
        public Guid ReviewId { get; init; }
        public Guid UserId { get; init; }
        public VoteType VoteType { get; init; }

        public VoteOnProductReviewCommand(Guid productId, Guid reviewId, Guid userId, VoteType voteType)
        {
            ProductId = productId;
            ReviewId = reviewId;
            UserId = userId;
            VoteType = voteType;
        }
    }
}
