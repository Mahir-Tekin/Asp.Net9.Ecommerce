using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Commands.DeleteVariationType
{
    public class DeleteVariationTypeCommandHandler : IRequestHandler<DeleteVariationTypeCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteVariationTypeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteVariationTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var variationType = await _unitOfWork.VariationTypes.GetByIdAsync(request.Id, cancellationToken);
                if (variationType == null)
                    return Result.NotFound($"Variation type with ID '{request.Id}' was not found");

                // TODO: Add business rules for deletion if needed
                // For example, check if the variation type is used by any products

                _unitOfWork.VariationTypes.Delete(variationType);
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