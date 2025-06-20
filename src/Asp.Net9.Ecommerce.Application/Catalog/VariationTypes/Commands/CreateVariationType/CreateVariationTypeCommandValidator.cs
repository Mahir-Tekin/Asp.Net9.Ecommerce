using FluentValidation;

namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.Commands.CreateVariationType
{
    public class CreateVariationTypeCommandValidator : AbstractValidator<CreateVariationTypeCommand>
    {
        public CreateVariationTypeCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters");

            RuleFor(x => x.DisplayName)
                .NotEmpty().WithMessage("Display name is required")
                .MaximumLength(100).WithMessage("Display name cannot exceed 100 characters");

            RuleForEach(x => x.Options)
                .ChildRules(option =>
                {
                    option.RuleFor(x => x.Value)
                        .NotEmpty().WithMessage("Option value is required")
                        .MaximumLength(50).WithMessage("Option value cannot exceed 50 characters");

                    option.RuleFor(x => x.DisplayValue)
                        .NotEmpty().WithMessage("Option display value is required")
                        .MaximumLength(100).WithMessage("Option display value cannot exceed 100 characters");
                });

            // Validate no duplicate option values
            RuleFor(x => x.Options)
                .Must(options => options.Select(o => o.Value.ToLowerInvariant().Trim())
                    .Distinct()
                    .Count() == options.Count)
                .WithMessage("Duplicate option values are not allowed");
        }
    }
} 