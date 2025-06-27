using Asp.Net9.Ecommerce.Application.Orders.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Orders.Commands
{
    public class CreateOrderCommand : IRequest<Result<Guid>>
    {
        public CreateOrderDto Order { get; set; }
        public Guid UserId { get; set; }
        public CreateOrderCommand(CreateOrderDto order, Guid userId)
        {
            Order = order;
            UserId = userId;
        }
    }
}
