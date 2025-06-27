using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Application.Orders.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asp.Net9.Ecommerce.Application.Orders.Queries
{
    public class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, Result<List<OrderSummaryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetUserOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<List<OrderSummaryDto>>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.Orders
                .GetQueryable()
                .Where(o => o.UserId == request.UserId)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(o => o.Items)
                .ToListAsync(cancellationToken);

            var dtos = orders.Select(order => new OrderSummaryDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                Items = order.Items.Select(item => new DTOs.OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductVariantId = item.ProductVariantId,
                    ProductName = item.ProductName,
                    ProductSlug = item.ProductSlug,
                    VariantName = item.VariantName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    ImageUrl = item.ImageUrl
                }).ToList()
            }).ToList();

            return Result.Success(dtos);
        }
    }
}
