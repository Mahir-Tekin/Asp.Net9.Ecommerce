using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Domain.Catalog;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                // Check if slug is unique
                if (await _unitOfWork.Categories.ExistsBySlugAsync(request.Slug, cancellationToken))
                {
                    return Result.Failure<Guid>(ErrorResponse.Conflict($"Category with slug '{request.Slug}' already exists"));
                }

                // Check if parent category exists if specified
                if (request.ParentCategoryId.HasValue)
                {
                    var parentExists = await _unitOfWork.Categories.ExistsByIdAsync(request.ParentCategoryId.Value, cancellationToken);
                    if (!parentExists)
                    {
                        return Result.Failure<Guid>(ErrorResponse.NotFound("Parent category not found"));
                    }
                }

                // Validate variation types exist and fetch them
                var variationTypeIds = request.VariationTypes.Select(vt => vt.VariationTypeId).ToList();
                var variationTypes = await _unitOfWork.VariationTypes.GetByIdsAsync(variationTypeIds, cancellationToken);
                if (variationTypes.Count != variationTypeIds.Count)
                {
                    return Result.Failure<Guid>(ErrorResponse.NotFound("One or more variation types not found"));
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

                // Add variation types to the category (many-to-many)
                foreach (var variationType in variationTypes)
                {
                    var addResult = category.AddVariationType(variationType);
                    if (addResult.IsFailure)
                        return Result.Failure<Guid>(addResult.Error);
                }

                _unitOfWork.Categories.Add(category);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result.Success(category.Id);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}