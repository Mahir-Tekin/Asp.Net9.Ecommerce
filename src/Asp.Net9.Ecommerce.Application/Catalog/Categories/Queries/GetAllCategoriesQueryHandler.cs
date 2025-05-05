using Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Queries
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<List<CategoryNestedDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllCategoriesQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<CategoryNestedDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.Categories.GetCategoryTreeAsync(cancellationToken);
            var result = _mapper.Map<List<CategoryNestedDto>>(categories);
            return Result.Success(result);
        }
    }
} 