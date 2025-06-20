using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Queries.GetVariationTypeById
{
    public class GetVariationTypeByIdQueryHandler : IRequestHandler<GetVariationTypeByIdQuery, Result<VariationTypeResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetVariationTypeByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<VariationTypeResponse>> Handle(GetVariationTypeByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var variationType = await _unitOfWork.VariationTypes.GetByIdAsync(request.Id, cancellationToken);
                if (variationType == null)
                    return Result.NotFound<VariationTypeResponse>($"Variation type with ID '{request.Id}' was not found");

                var response = _mapper.Map<VariationTypeResponse>(variationType);
                return Result.Success(response);
            }
            catch (Exception ex)
            {
                return Result.Failure<VariationTypeResponse>(ErrorResponse.Internal(ex.Message));
            }
        }
    }
} 