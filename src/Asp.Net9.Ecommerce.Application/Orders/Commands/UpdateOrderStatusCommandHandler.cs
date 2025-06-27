using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Asp.Net9.Ecommerce.Application.Orders.Commands
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateOrderStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
                return Result.NotFound("Order not found");

            if (!Enum.TryParse<Domain.Orders.OrderStatus>(request.NewStatus, true, out var newStatus))
                return Result.ValidationFailure(new List<ValidationError> { new("NewStatus", "Invalid order status value") });

            order.SetStatus(newStatus);
            await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
