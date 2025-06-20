using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Queries.GetAllVariationTypes
{
    public class GetAllVariationTypesQueryHandler : IRequestHandler<GetAllVariationTypesQuery, Result<List<VariationTypeResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllVariationTypesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<VariationTypeResponse>>> Handle(GetAllVariationTypesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var variationTypes = await _unitOfWork.VariationTypes.GetAllAsync(cancellationToken);
                var response = _mapper.Map<List<VariationTypeResponse>>(variationTypes);
                return Result.Success(response);
            }
            catch (Exception ex)
            {
                return Result.Failure<List<VariationTypeResponse>>(ErrorResponse.Internal(ex.Message));
            }
        }
    }
} 