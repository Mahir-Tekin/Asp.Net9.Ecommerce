using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Application.Orders.DTOs;
using Asp.Net9.Ecommerce.Domain.Orders;
using AutoMapper;
using MediatR;
using System.Security.Claims;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Application.Orders.Commands
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            const int maxRetries = 3;
            int retryCount = 0;
            while (true)
            {
                try
                {
                    await _unitOfWork.BeginTransactionAsync(cancellationToken);

                    // 1. Collect all variant IDs from the order request
                    var variantIds = request.Order.Items.Select(i => i.ProductVariantId).ToList();
                    // 2. Fetch all variants in a single call
                    var variants = await _unitOfWork.Products.GetVariantsByIdsAsync(variantIds, cancellationToken);
                    // 3. Check if all variants exist
                    if (variants.Count != variantIds.Count)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result.Failure<Guid>(ErrorResponse.NotFound("One or more product variants not found."));
                    }

                    // 4. Check stock for each item and decrease stock using domain method
                    foreach (var item in request.Order.Items)
                    {
                        var variant = variants.First(v => v.Id == item.ProductVariantId);
                        if (variant.TrackInventory)
                        {
                            if (variant.StockQuantity < item.Quantity)
                            {
                                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                                return Result.Failure<Guid>(ErrorResponse.Conflict($"Not enough stock for product: {variant.Name} (SKU: {variant.SKU})"));
                            }
                            // Use domain method to decrease stock (implement in ProductVariant if not present)
                            variant.DecreaseStock(item.Quantity);
                        }
                    }

                    // 5. Calculate backend prices and total
                    var orderItems = new List<OrderItem>();
                    decimal totalAmount = 0;
                    foreach (var item in request.Order.Items)
                    {
                        var variant = variants.First(v => v.Id == item.ProductVariantId);
                        var unitPrice = variant.Price;
                        var orderItem = new OrderItem(
                            variant.ProductId,
                            variant.Id,
                            variant.Name ?? string.Empty,
                            variant.Product?.Slug ?? string.Empty,
                            item.VariantName,
                            item.Quantity,
                            unitPrice,
                            item.ImageUrl
                        );
                        orderItems.Add(orderItem);
                        totalAmount += unitPrice * item.Quantity;
                    }

                    // 6. Create order entity using backend-calculated data
                    var order = new Order(request.UserId, _mapper.Map<OrderAddress>(request.Order.ShippingAddress), orderItems);
                    // Optionally, set total explicitly if needed
                    // order.TotalAmount = totalAmount;

                    // 7. Add order to repository and save
                    await _unitOfWork.Orders.AddAsync(order, cancellationToken);
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    return Result.Success(order.Id);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    if (++retryCount >= maxRetries)
                    {
                        return Result.Failure<Guid>(ErrorResponse.Conflict("A concurrency error occurred while creating the order. Please try again."));
                    }
                    // else, retry
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<Guid>(ErrorResponse.Internal(ex.Message));
                }
            }
        }
    }
}
