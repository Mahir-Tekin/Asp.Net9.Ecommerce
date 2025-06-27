using Asp.Net9.Ecommerce.Application.Orders.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;
using System;
using System.Collections.Generic;

namespace Asp.Net9.Ecommerce.Application.Orders.Queries
{
    public class GetUserOrdersQuery : IRequest<Result<List<OrderSummaryDto>>>
    {
        public Guid UserId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
