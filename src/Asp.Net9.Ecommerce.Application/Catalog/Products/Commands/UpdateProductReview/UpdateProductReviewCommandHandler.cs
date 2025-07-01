using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.UpdateProductReview
{
    public class UpdateProductReviewCommandHandler : IRequestHandler<UpdateProductReviewCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductReviewCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateProductReviewCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                // 1. Verify the product exists
                var productExists = await _unitOfWork.Products.ExistsAsync(request.ProductId, cancellationToken);
                if (!productExists)
                    return Result.NotFound("Product not found");

                // 2. Get product with reviews to access the specific review
                var product = await _unitOfWork.Products.GetByIdWithReviewsAsync(request.ProductId, cancellationToken);
                if (product == null)
                    return Result.NotFound("Product not found");

                // 3. Update the review using the product aggregate
                var updateResult = product.UpdateReview(
                    request.ReviewId,
                    request.UserId,
                    request.Review.Rating,
                    request.Review.Title,
                    request.Review.Comment);

                if (updateResult.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return updateResult;
                }

                // 4. Save changes
                var saveResult = await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
                if (saveResult.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return saveResult;
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure(ErrorResponse.Internal($"An error occurred while updating the review: {ex.Message}"));
            }
        }
    }
}
