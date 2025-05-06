using FluentValidation;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Product ID is required");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(200)
                .WithMessage("Name cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .When(x => !string.IsNullOrEmpty(x.Description))
                .WithMessage("Description cannot exceed 2000 characters");

            RuleFor(x => x.BasePrice)
                .GreaterThan(0)
                .WithMessage("Base price must be greater than zero");

            RuleFor(x => x.Variants)
                .NotEmpty()
                .WithMessage("At least one variant is required");

            RuleForEach(x => x.Variants).SetValidator(new ProductVariantUpdateInfoValidator());
        }
    }

    public class ProductVariantUpdateInfoValidator : AbstractValidator<ProductVariantUpdateInfo>
    {
        public ProductVariantUpdateInfoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Variant ID is required");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(200)
                .WithMessage("Name cannot exceed 200 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .When(x => x.Price.HasValue)
                .WithMessage("Price must be greater than zero");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0)
                .When(x => x.StockQuantity.HasValue)
                .WithMessage("Stock quantity cannot be negative");

            RuleFor(x => x.Variations)
                .NotNull()
                .WithMessage("Variations dictionary cannot be null");
        }
    }
} 