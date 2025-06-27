using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Domain.Catalog;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken);
                
                if (category == null)
                    return Result.Failure(ErrorResponse.NotFound("Category not found."));

                // Check if slug is being changed and if it's already in use
                if (category.Slug != request.Slug && 
                    await _unitOfWork.Categories.ExistsBySlugAsync(request.Slug, cancellationToken))
                {
                    return Result.Failure(ErrorResponse.Conflict($"A category with slug '{request.Slug}' already exists."));
                }

                // Validate that all variation types exist
                foreach (var variationType in request.VariationTypes)
                {
                    if (!await _unitOfWork.VariationTypes.ExistsByIdAsync(variationType.VariationTypeId, cancellationToken))
                    {
                        return Result.Failure(ErrorResponse.NotFound($"Variation type with ID '{variationType.VariationTypeId}' not found."));
                    }
                }

                var updateResult = category.Update(request.Name, request.Description, request.Slug);
                if (updateResult.IsFailure)
                    return updateResult;

                // Update variation types
                var variationTypeIds = request.VariationTypes.Select(vt => vt.VariationTypeId).ToList();
                var variationTypes = await _unitOfWork.VariationTypes.GetByIdsAsync(variationTypeIds, cancellationToken);
                if (variationTypes.Count != variationTypeIds.Count)
                {
                    return Result.Failure(ErrorResponse.NotFound("One or more variation types not found"));
                }
                var updateVariationTypesResult = category.UpdateVariationTypes(variationTypes);
                if (updateVariationTypesResult.IsFailure)
                    return updateVariationTypesResult;

                // Handle IsActive state change
                if (category.IsActive != request.IsActive)
                {
                    var stateChangeResult = request.IsActive ? category.Activate() : category.Deactivate();
                    if (stateChangeResult.IsFailure)
                        return stateChangeResult;
                }

                _unitOfWork.Categories.Update(category);
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