using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Application.Orders.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asp.Net9.Ecommerce.Application.Orders.Queries
{
    public class GetUserOrderByIdQueryHandler : IRequestHandler<GetUserOrderByIdQuery, Result<OrderSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetUserOrderByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<OrderSummaryDto>> Handle(GetUserOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders
                .GetQueryable()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, cancellationToken);
            if (order == null)
                return Result.NotFound<OrderSummaryDto>("Order not found");
            var dto = new OrderSummaryDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                Items = order.Items.Select(item => new OrderItemDto
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
            };
            return Result.Success(dto);
        }
    }
}
