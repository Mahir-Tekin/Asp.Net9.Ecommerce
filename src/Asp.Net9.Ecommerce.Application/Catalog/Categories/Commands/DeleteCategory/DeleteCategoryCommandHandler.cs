using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken);
                
                if (category == null)
                    return Result.Failure(ErrorResponse.NotFound("Category not found."));

                // Check if category has subcategories
                var hasSubCategories = category.SubCategories.Any();
                if (hasSubCategories)
                    return Result.Failure(ErrorResponse.General("Cannot delete a category that has subcategories.", "CATEGORY_HAS_SUBCATEGORIES"));

                _unitOfWork.Categories.Delete(category);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result.Success();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
} 