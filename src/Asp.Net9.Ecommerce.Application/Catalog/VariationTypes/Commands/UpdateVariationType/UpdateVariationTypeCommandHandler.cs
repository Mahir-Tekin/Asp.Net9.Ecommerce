using Asp.Net9.Ecommerce.Application.Common.Interfaces;
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
                // Get existing variation type
                var variationType = await _unitOfWork.VariationTypes.GetByIdAsync(request.Id, cancellationToken);
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

                // Update options
                foreach (var option in request.Options.OrderBy(o => o.SortOrder))
                {
                    // Remove existing option if it exists
                    var removeResult = variationType.RemoveOption(option.Value);
                    
                    // Add the new/updated option
                    var addResult = variationType.AddOption(option.Value, option.DisplayValue);
                    if (addResult.IsFailure)
                        return addResult;
                }

                // Save changes
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