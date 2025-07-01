using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Domain.Catalog;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Commands.UpdateVariationType
{
    public class UpdateVariationTypeCommandHandler : IRequestHandler<UpdateVariationTypeCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateVariationTypeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateVariationTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get existing variation type with options
                var variationType = await _unitOfWork.VariationTypes.GetByIdWithOptionsAsync(request.Id, cancellationToken);
                if (variationType == null)
                    return Result.NotFound($"Variation type with ID '{request.Id}' was not found");

                // Check if name is already taken by another variation type
                if (request.Name != variationType.Name)
                {
                    var existingType = await _unitOfWork.VariationTypes.GetByNameAsync(request.Name, cancellationToken);
                    if (existingType != null && existingType.Id != request.Id)
                        return Result.Failure(ErrorResponse.Conflict($"A variation type with name '{request.Name}' already exists"));
                }

                // Update basic properties
                var updateResult = variationType.UpdateName(request.Name, request.DisplayName);
                if (updateResult.IsFailure)
                    return updateResult;

                // Update active status
                if (request.IsActive && !variationType.IsActive)
                {
                    var activateResult = variationType.Activate();
                    if (activateResult.IsFailure)
                        return activateResult;
                }
                else if (!request.IsActive && variationType.IsActive)
                {
                    var deactivateResult = variationType.Deactivate();
                    if (deactivateResult.IsFailure)
                        return deactivateResult;
                }

                // Update options safely - preserve existing options to maintain product variant relationships
                // IMPORTANT: We only update existing options and add new ones using domain methods
                // We DON'T remove existing options automatically because product variants depend on them.

                // First, handle all existing option updates
                foreach (var requestOption in request.Options.Where(o => o.Id.HasValue).OrderBy(o => o.SortOrder))
                {
                    // Update existing option - this preserves the option ID and all relationships
                    var updateOptionResult = variationType.UpdateOption(
                        requestOption.Id!.Value, 
                        requestOption.Value, 
                        requestOption.DisplayValue, 
                        requestOption.SortOrder);
                    
                    if (updateOptionResult.IsFailure)
                        return updateOptionResult;
                }

                // Then, handle new options by creating them separately and adding to DbContext directly
                var newOptions = request.Options.Where(o => !o.Id.HasValue).OrderBy(o => o.SortOrder).ToList();
                
                foreach (var requestOption in newOptions)
                {
                    // Create new option with proper entity state
                    var optionResult = VariantOption.Create(
                        requestOption.Value, 
                        requestOption.DisplayValue, 
                        requestOption.SortOrder, 
                        variationType.Id);
                    
                    if (optionResult.IsFailure)
                        return Result.Failure(optionResult.Error);

                    // Add the new option to the collection - EF Core will track it as Added
                    variationType.Options.Add(optionResult.Value);
                }

                // Save all changes in one transaction
                _unitOfWork.VariationTypes.Update(variationType);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ErrorResponse.Internal(ex.Message));
            }
        }
    }
}
