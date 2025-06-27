using Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Queries.GetCategoryFilters
{
    public class GetCategoryFiltersQueryHandler : IRequestHandler<GetCategoryFiltersQuery, Result<CategoryFiltersResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCategoryFiltersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CategoryFiltersResponse>> Handle(GetCategoryFiltersQuery request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetCategoryByIdAsync(request.CategoryId, cancellationToken);
            
            if (category == null)
                return Result.Failure<CategoryFiltersResponse>(ErrorResponse.NotFound("Category not found"));

            var response = new CategoryFiltersResponse
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                VariationTypes = _mapper.Map<List<Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs.VariationTypeDto>>(category.VariationTypes)
            };

            return Result.Success(response);
        }
    }
}
