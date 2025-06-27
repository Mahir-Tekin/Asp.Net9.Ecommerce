using Asp.Net9.Ecommerce.Application.Orders.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;
using System;

namespace Asp.Net9.Ecommerce.Application.Orders.Queries
{
    public class GetAdminOrderByIdQuery : IRequest<Result<OrderSummaryDto>>
    {
        public Guid OrderId { get; set; }
    }
}
