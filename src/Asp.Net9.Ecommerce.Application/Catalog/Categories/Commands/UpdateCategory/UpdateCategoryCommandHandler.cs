using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result>
    {
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (category == null)
                return Result.Failure(ErrorResponse.NotFound("Category not found."));

            // Check if slug is being changed and if it's already in use
            if (category.Slug != request.Slug && 
                await _categoryRepository.ExistsBySlugAsync(request.Slug, cancellationToken))
            {
                return Result.Failure(ErrorResponse.Conflict($"A category with slug '{request.Slug}' already exists."));
            }

            var updateResult = category.Update(request.Name, request.Description, request.Slug);
            if (updateResult.IsFailure)
                return updateResult;

            // Handle IsActive state change
            if (category.IsActive != request.IsActive)
            {
                var stateChangeResult = request.IsActive ? category.Activate() : category.Deactivate();
                if (stateChangeResult.IsFailure)
                    return stateChangeResult;
            }

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
} 