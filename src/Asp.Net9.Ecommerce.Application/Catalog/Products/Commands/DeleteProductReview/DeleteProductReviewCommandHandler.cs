
using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.DeleteProductReview;

public sealed class DeleteProductReviewCommandHandler : IRequestHandler<DeleteProductReviewCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductReviewCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteProductReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // 1. Check if the product exists
            if (!await _unitOfWork.Products.ExistsAsync(request.ProductId, cancellationToken))
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.NotFound("Product not found");
            }

            // 2. Get product with reviews to access the specific review
            var product = await _unitOfWork.Products.GetByIdWithReviewsAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.NotFound("Product not found");
            }

            // 3. Remove the review from the product (domain handles business rules)
            var removeResult = product.RemoveReview(request.ReviewId, request.UserId);
            if (removeResult.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return removeResult;
            }

            // 4. Save changes
            var saveResult = await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
            if (saveResult.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return saveResult;
            }

            // 5. Commit the transaction
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
