using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                // 1. Get product
                var product = await _unitOfWork.Products.GetByIdWithVariantsAsync(request.Id, cancellationToken);
                if (product == null)
                    return Result.NotFound("Product not found");

                // 2. Delete product (soft delete)
                var deleteResult = product.Delete();
                if (deleteResult.IsFailure)
                    return deleteResult;

                // 3. Update in repository
                var updateResult = await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
                if (updateResult.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return updateResult;
                }

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure(ErrorResponse.General(
                    $"An error occurred while deleting the product: {ex.Message}",
                    "PRODUCT_DELETE_ERROR"));
            }
        }
    }
} 