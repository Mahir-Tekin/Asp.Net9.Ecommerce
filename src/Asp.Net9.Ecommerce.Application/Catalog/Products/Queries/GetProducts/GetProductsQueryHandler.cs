using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProducts
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<ProductListResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ProductListResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var (products, totalCount) = await _unitOfWork.Products.GetProductListAsync(
                request.SearchTerm,
                request.CategoryId,
                request.MinPrice,
                request.MaxPrice,
                request.HasStock,
                request.IsActive,
                request.VariationFilters,
                request.SortBy,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            var productDtos = _mapper.Map<List<ProductListDto>>(products);

            var response = new ProductListResponse
            {
                Items = productDtos,
                TotalItems = totalCount,
                CurrentPage = request.PageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                HasPreviousPage = request.PageNumber > 1,
                HasNextPage = request.PageNumber < (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };

            return Result.Success(response);
        }
    }
} 