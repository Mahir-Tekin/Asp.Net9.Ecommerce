using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProductBySlug
{
    public class GetProductBySlugQueryHandler : IRequestHandler<GetProductBySlugQuery, Result<ProductDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductBySlugQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ProductDetailsDto>> Handle(GetProductBySlugQuery request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetBySlugWithDetailsAsync(request.Slug, cancellationToken);
            if (product == null)
                return Result.NotFound<ProductDetailsDto>("Product not found");

            var dto = _mapper.Map<ProductDetailsDto>(product);
            return Result.Success(dto);
        }
    }
}
