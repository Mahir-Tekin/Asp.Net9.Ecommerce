using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Application.Orders.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asp.Net9.Ecommerce.Application.Orders.Queries
{
    public class GetAdminOrderByIdQueryHandler : IRequestHandler<GetAdminOrderByIdQuery, Result<OrderSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminOrderByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<OrderSummaryDto>> Handle(GetAdminOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders
                .GetQueryable()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
            if (order == null)
                return Result.NotFound<OrderSummaryDto>("Order not found");
            var dto = new OrderSummaryDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                ShippingAddress = new OrderAddressDto
                {
                    FirstName = order.ShippingAddress.FirstName,
                    LastName = order.ShippingAddress.LastName,
                    PhoneNumber = order.ShippingAddress.PhoneNumber,
                    City = order.ShippingAddress.City,
                    District = order.ShippingAddress.District,
                    Neighborhood = order.ShippingAddress.Neighborhood,
                    AddressLine = order.ShippingAddress.AddressLine,
                    AddressTitle = order.ShippingAddress.AddressTitle
                },
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
