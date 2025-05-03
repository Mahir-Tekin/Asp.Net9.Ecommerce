using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly ICategoryRepository _categoryRepository;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (category == null)
                return Result.Failure(ErrorResponse.NotFound("Category not found."));

            // Check if category has subcategories
            var hasSubCategories = category.SubCategories.Any();
            if (hasSubCategories)
                return Result.Failure(ErrorResponse.General("Cannot delete a category that has subcategories.", "CATEGORY_HAS_SUBCATEGORIES"));

            _categoryRepository.Delete(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
} 