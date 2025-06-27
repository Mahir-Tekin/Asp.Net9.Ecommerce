using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;
using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ProductDetailsDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdWithDetailsAsync(request.Id, cancellationToken);
            if (product == null)
                return Result.NotFound<ProductDetailsDto>("Product not found");

            var productDto = _mapper.Map<ProductDetailsDto>(product);
            return Result.Success(productDto);
        }
    }
} 