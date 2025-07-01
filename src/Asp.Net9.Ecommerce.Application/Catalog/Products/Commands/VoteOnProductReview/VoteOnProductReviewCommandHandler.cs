using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Domain.Catalog;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.VoteOnProductReview
{
    public class VoteOnProductReviewCommandHandler : IRequestHandler<VoteOnProductReviewCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public VoteOnProductReviewCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(VoteOnProductReviewCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                // 1. Verify the product exists
                var productExists = await _unitOfWork.Products.ExistsAsync(request.ProductId, cancellationToken);
                if (!productExists)
                    return Result.NotFound("Product not found");

                // 2. Get product with reviews to verify review exists and get the review entity
                var product = await _unitOfWork.Products.GetByIdWithReviewsAsync(request.ProductId, cancellationToken);
                if (product == null)
                    return Result.NotFound("Product not found");

                var review = product.Reviews.FirstOrDefault(r => r.Id == request.ReviewId && !r.IsDeleted);
                if (review == null)
                    return Result.NotFound("Review not found");

                // 3. Check if user is trying to vote on their own review
                if (review.UserId == request.UserId)
                    return Result.Failure(ErrorResponse.ValidationError(
                        new List<ValidationError> { new("Vote", "You cannot vote on your own review") }));

                // 4. Check if user has already voted on this review
                var existingVote = await _unitOfWork.Products.GetVoteByReviewAndUserAsync(request.ReviewId, request.UserId, cancellationToken);

                if (existingVote != null)
                {
                    // User has already voted - check if they're trying to change their vote
                    if (existingVote.VoteType == request.VoteType)
                    {
                        // Same vote type - remove the vote (toggle off)
                        var removeVoteResult = review.RemoveVote(existingVote.VoteType);
                        if (removeVoteResult.IsFailure)
                            return removeVoteResult;

                        await _unitOfWork.Products.RemoveVoteAsync(existingVote, cancellationToken);
                    }
                    else
                    {
                        // Different vote type - change the vote
                        var changeVoteResult = review.ChangeVote(existingVote.VoteType, request.VoteType);
                        if (changeVoteResult.IsFailure)
                            return changeVoteResult;

                        existingVote.UpdateVoteType(request.VoteType);
                        await _unitOfWork.Products.UpdateVoteAsync(existingVote, cancellationToken);
                    }
                }
                else
                {
                    // User hasn't voted yet - add new vote
                    if (!review.CanUserVote(request.UserId))
                        return Result.Failure(ErrorResponse.ValidationError(
                            new List<ValidationError> { new("Vote", "You cannot vote on your own review") }));

                    var newVoteResult = ReviewVote.Create(request.ReviewId, request.UserId, request.VoteType);
                    if (newVoteResult.IsFailure)
                        return Result.Failure(newVoteResult.Error);

                    var addVoteResult = review.AddVote(request.VoteType);
                    if (addVoteResult.IsFailure)
                        return addVoteResult;

                    await _unitOfWork.Products.AddVoteAsync(newVoteResult.Value, cancellationToken);
                }

                // 5. Save changes
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure(ErrorResponse.Internal($"An error occurred while processing the vote: {ex.Message}"));
            }
        }
    }
}
