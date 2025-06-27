using FluentValidation;

namespace Asp.Net9.Ecommerce.Application.Authentication.Commands.Address
{
    public class CreateAddressCommandValidator : AbstractValidator<CreateAddressCommand>
    {
        public CreateAddressCommandValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(30);
            RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.District).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Neighborhood).NotEmpty().MaximumLength(100);
            RuleFor(x => x.AddressLine).NotEmpty().MaximumLength(200);
            RuleFor(x => x.AddressTitle).NotEmpty().MaximumLength(100);
        }
    }
}
