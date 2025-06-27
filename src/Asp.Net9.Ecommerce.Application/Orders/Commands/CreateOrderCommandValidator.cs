using Asp.Net9.Ecommerce.Application.Orders.DTOs;
using FluentValidation;

namespace Asp.Net9.Ecommerce.Application.Orders.Commands
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.Order).NotNull();
            RuleFor(x => x.Order.ShippingAddress).NotNull();
            RuleFor(x => x.Order.ShippingAddress.FirstName).NotEmpty();
            RuleFor(x => x.Order.ShippingAddress.LastName).NotEmpty();
            RuleFor(x => x.Order.ShippingAddress.PhoneNumber).NotEmpty();
            RuleFor(x => x.Order.ShippingAddress.City).NotEmpty();
            RuleFor(x => x.Order.ShippingAddress.District).NotEmpty();
            RuleFor(x => x.Order.ShippingAddress.Neighborhood).NotEmpty();
            RuleFor(x => x.Order.ShippingAddress.AddressLine).NotEmpty();
            RuleFor(x => x.Order.ShippingAddress.AddressTitle).NotEmpty();
            RuleFor(x => x.Order.Items).NotNull().NotEmpty();
            RuleForEach(x => x.Order.Items).SetValidator(new OrderItemDtoValidator());
        }
    }

    public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
    {
        public OrderItemDtoValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.ProductVariantId).NotEmpty();
            RuleFor(x => x.ProductName).NotEmpty();
            RuleFor(x => x.ProductSlug).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.UnitPrice).GreaterThan(0);
        }
    }
}
