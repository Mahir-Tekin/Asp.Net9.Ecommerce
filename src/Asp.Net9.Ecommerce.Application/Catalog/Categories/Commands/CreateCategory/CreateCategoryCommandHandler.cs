using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Domain.Catalog;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            // Check if slug is unique
            if (await _categoryRepository.ExistsBySlugAsync(request.Slug, cancellationToken))
            {
                return Result.Failure<Guid>(ErrorResponse.Conflict($"Category with slug '{request.Slug}' already exists"));
            }

            // Check if parent category exists if specified
            if (request.ParentCategoryId.HasValue)
            {
                var parentExists = await _categoryRepository.ExistsByIdAsync(request.ParentCategoryId.Value, cancellationToken);
                if (!parentExists)
                {
                    return Result.Failure<Guid>(ErrorResponse.NotFound("Parent category not found"));
                }
            }

            // Create category using domain factory method
            var categoryResult = Category.Create(
                request.Name,
                request.Description,
                request.Slug,
                request.ParentCategoryId);

            if (categoryResult.IsFailure)
                return Result.Failure<Guid>(categoryResult.Error);

            var category = categoryResult.Value;

            _categoryRepository.Add(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);

            return Result.Success(category.Id);
        }
    }
} 