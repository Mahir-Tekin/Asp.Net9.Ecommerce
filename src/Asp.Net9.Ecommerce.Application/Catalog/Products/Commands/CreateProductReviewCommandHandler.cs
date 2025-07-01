using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Domain.Catalog;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands
{
    public class CreateProductReviewCommandHandler : IRequestHandler<CreateProductReviewCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductReviewCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateProductReviewCommand request, CancellationToken cancellationToken)
        {
            // 1. Check if the user has purchased and received the product
            var hasPurchased = await _unitOfWork.Orders.HasUserPurchasedAndReceivedProductAsync(
                request.UserId, request.Dto.ProductId, cancellationToken);

            if (!hasPurchased)
            {
                return Result.Failure<Guid>(ErrorResponse.General(
                    "You can only review products you have purchased and received.",
                    "REVIEW_NOT_ELIGIBLE"));
            }

            // 2. Check if the user has already reviewed this product
            var hasReviewed = await _unitOfWork.Products.HasUserReviewedProductAsync(
                request.UserId, request.Dto.ProductId, cancellationToken);

            if (hasReviewed)
            {
                return Result.Failure<Guid>(ErrorResponse.General(
                    "You have already reviewed this product.",
                    "REVIEW_ALREADY_EXISTS"));
            }

            // 3. Validate that the product exists
            var productExists = await _unitOfWork.Products.ExistsAsync(request.Dto.ProductId, cancellationToken);
            if (!productExists)
            {
                return Result.Failure<Guid>(ErrorResponse.NotFound("Product not found."));
            }

            // 4. Create the review using the domain factory method
            var reviewResult = ProductReview.Create(
                productId: request.Dto.ProductId,
                userId: request.UserId,
                rating: request.Dto.Rating,
                title: request.Dto.Title,
                comment: request.Dto.Comment);

            if (reviewResult.IsFailure)
            {
                return Result.Failure<Guid>(reviewResult.Error);
            }

            var review = reviewResult.Value;

            // 5. Add the review to the context directlyupdate
            await _unitOfWork.Products.AddReviewAsync(review, cancellationToken);

            // 6. Save changes first
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 7. Now update product's computed review statistics after the review is persisted
            var product = await _unitOfWork.Products.GetByIdWithReviewsAsync(request.Dto.ProductId, cancellationToken);
            if (product != null)
            {
                product.RecalculateReviewStatistics();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Result.Success(review.Id);
        }
    }
}
