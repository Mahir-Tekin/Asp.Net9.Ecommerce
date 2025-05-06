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

            // Variant types validation
            When(x => x.VariantTypes != null && x.VariantTypes.Any(), () =>
            {
                RuleFor(x => x.VariantTypes)
                    .ForEach(type => type.SetValidator(new VariantTypeInfoValidator()));

                // If variant types are provided, ensure all variants have the required variations
                RuleForEach(x => x.Variants)
                    .Must((command, variant) => HasRequiredVariations(variant, command.VariantTypes))
                    .WithMessage((command, variant) => 
                        $"Variant with SKU '{variant.SKU}' is missing required variations for the specified variant types");
            });
        }

        private bool HasRequiredVariations(ProductVariantInfo variant, List<VariantTypeInfo> variantTypes)
        {
            if (variant.Variations == null)
                return false;

            return variantTypes.All(type => 
                variant.Variations.ContainsKey(type.Name) && 
                !string.IsNullOrWhiteSpace(variant.Variations[type.Name]));
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

            RuleFor(x => x.Variations)
                .NotNull().WithMessage("Variations dictionary cannot be null");
        }
    }

    public class VariantTypeInfoValidator : AbstractValidator<VariantTypeInfo>
    {
        public VariantTypeInfoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Variant type name is required")
                .MaximumLength(50).WithMessage("Variant type name cannot exceed 50 characters")
                .Matches("^[a-z0-9-]+$").WithMessage("Variant type name must be lowercase and can only contain letters, numbers, and hyphens");

            RuleFor(x => x.DisplayName)
                .NotEmpty().WithMessage("Display name is required")
                .MaximumLength(100).WithMessage("Display name cannot exceed 100 characters");

            RuleFor(x => x.Options)
                .NotEmpty().WithMessage("At least one option is required")
                .ForEach(option => option.SetValidator(new VariantOptionInfoValidator()));
        }
    }

    public class VariantOptionInfoValidator : AbstractValidator<VariantOptionInfo>
    {
        public VariantOptionInfoValidator()
        {
            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Option value is required")
                .MaximumLength(50).WithMessage("Option value cannot exceed 50 characters")
                .Matches("^[a-z0-9-]+$").WithMessage("Option value must be lowercase and can only contain letters, numbers, and hyphens");

            RuleFor(x => x.DisplayValue)
                .NotEmpty().WithMessage("Display value is required")
                .MaximumLength(100).WithMessage("Display value cannot exceed 100 characters");

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Sort order cannot be negative");
        }
    }
} 