using FluentValidation;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("Slug is required")
                .MaximumLength(100).WithMessage("Slug must not exceed 100 characters")
                .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
                .WithMessage("Slug must be in valid format (lowercase letters, numbers, and hyphens only)");

            RuleForEach(x => x.VariationTypes)
                .ChildRules(variationType =>
                {
                    variationType.RuleFor(x => x.VariationTypeId)
                        .NotEmpty().WithMessage("Variation type ID is required");
                });
        }
    }
} 