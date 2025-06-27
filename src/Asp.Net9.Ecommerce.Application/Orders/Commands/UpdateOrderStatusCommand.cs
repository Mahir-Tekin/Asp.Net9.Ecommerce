using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;
using System;

namespace Asp.Net9.Ecommerce.Application.Orders.Commands
{
    public class UpdateOrderStatusCommand : IRequest<Result>
    {
        public Guid OrderId { get; set; }
        public string NewStatus { get; set; }
        public string? Comment { get; set; }
    }
}
