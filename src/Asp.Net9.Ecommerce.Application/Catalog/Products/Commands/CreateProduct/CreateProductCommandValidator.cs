using FluentValidation;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            // Basic product information validation
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.BasePrice)
                .GreaterThan(0).WithMessage("Base price must be greater than zero");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category is required");

            // Variants validation
            RuleFor(x => x.Variants)
                .NotEmpty().WithMessage("At least one variant is required")
                .ForEach(variant => variant.SetValidator(new ProductVariantInfoValidator()));

            // Images validation
            When(x => x.Images != null && x.Images.Any(), () =>
            {
                RuleForEach(x => x.Images)
                    .SetValidator(new ProductImageInfoValidator());
            });
        }
    }

    public class ProductVariantInfoValidator : AbstractValidator<ProductVariantInfo>
    {
        public ProductVariantInfoValidator()
        {
            RuleFor(x => x.SKU)
                .NotEmpty().WithMessage("SKU is required")
                .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters")
                .Matches("^[A-Za-z0-9-_.]+$").WithMessage("SKU can only contain letters, numbers, hyphens, underscores, and dots");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Variant name is required")
                .MaximumLength(200).WithMessage("Variant name cannot exceed 200 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero")
                .When(x => x.Price.HasValue);

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative")
                .When(x => x.TrackInventory);

            RuleFor(x => x.SelectedOptions)
                .NotNull().WithMessage("SelectedOptions dictionary cannot be null");
        }
    }

    public class ProductImageInfoValidator : AbstractValidator<ProductImageInfo>
    {
        public ProductImageInfoValidator()
        {
            RuleFor(x => x.Url)
                .NotEmpty().WithMessage("Image URL is required");
            RuleFor(x => x.AltText)
                .MaximumLength(200).WithMessage("Alt text cannot exceed 200 characters");
        }
    }

} 