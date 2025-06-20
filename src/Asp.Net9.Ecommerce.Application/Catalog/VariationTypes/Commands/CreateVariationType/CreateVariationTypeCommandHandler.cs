using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Domain.Catalog;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Commands.CreateVariationType
{
    public class CreateVariationTypeCommandHandler : IRequestHandler<CreateVariationTypeCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateVariationTypeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateVariationTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if name is already taken
                if (await _unitOfWork.VariationTypes.ExistsByNameAsync(request.Name, cancellationToken))
                    return Result.Failure<Guid>(ErrorResponse.Conflict($"A variation type with name '{request.Name}' already exists"));

                // Create variation type
                var variationTypeResult = VariationType.Create(request.Name, request.DisplayName);
                if (variationTypeResult.IsFailure)
                    return Result.Failure<Guid>(variationTypeResult.Error);

                var variationType = variationTypeResult.Value;

                // Add options if any
                foreach (var option in request.Options.OrderBy(o => o.SortOrder))
                {
                    var addOptionResult = variationType.AddOption(option.Value, option.DisplayValue, option.SortOrder);
                    if (addOptionResult.IsFailure)
                        return Result.Failure<Guid>(addOptionResult.Error);
                }

                // Save to database
                _unitOfWork.VariationTypes.Add(variationType);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(variationType.Id);
            }
            catch (Exception ex)
            {
                return Result.Failure<Guid>(ErrorResponse.Internal(ex.Message));
            }
        }
    }
} 