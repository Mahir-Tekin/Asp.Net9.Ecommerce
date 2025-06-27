using Asp.Net9.Ecommerce.Application.Orders.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Orders.Queries
{
    public class GetAdminOrdersQuery : IRequest<Result<AdminOrderListResultDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Status { get; set; }
        public bool SortAsc { get; set; } = false;
    }
}
