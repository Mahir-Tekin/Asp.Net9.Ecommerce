using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Application.Orders.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Asp.Net9.Ecommerce.Application.Orders.Queries
{
    public class GetAdminOrdersQueryHandler : IRequestHandler<GetAdminOrdersQuery, Result<AdminOrderListResultDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminOrdersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<AdminOrderListResultDto>> Handle(GetAdminOrdersQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Orders.GetQueryable();
            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(o => o.Status.ToString() == request.Status);
            query = request.SortAsc
                ? query.OrderBy(o => o.CreatedAt)
                : query.OrderByDescending(o => o.CreatedAt);
            var totalCount = await query.CountAsync(cancellationToken);
            var orders = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(o => o.Items)
                .ToListAsync(cancellationToken);
            var result = new AdminOrderListResultDto
            {
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                Orders = orders.Select(o => new AdminOrderListItemDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount,
                    ItemCount = o.Items.Count
                }).ToList()
            };
            return Result.Success(result);
        }
    }
}
