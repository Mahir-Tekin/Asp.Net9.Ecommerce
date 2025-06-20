using Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Queries.GetAllCategoriesForAdmin
{
    public class GetAllCategoriesForAdminQueryHandler : IRequestHandler<GetAllCategoriesForAdminQuery, Result<List<CategoryNestedDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllCategoriesForAdminQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<CategoryNestedDto>>> Handle(GetAllCategoriesForAdminQuery request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
            
            // Get root categories (those without parent)
            var rootCategories = categories
                .Where(c => c.ParentCategoryId == null)
                .ToList();

            var result = _mapper.Map<List<CategoryNestedDto>>(rootCategories);
            return Result.Success(result);
        }
    }
} 