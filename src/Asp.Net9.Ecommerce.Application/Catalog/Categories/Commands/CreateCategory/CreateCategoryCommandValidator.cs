using FluentValidation;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("Slug is required")
                .MaximumLength(200).WithMessage("Slug must not exceed 200 characters")
                .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
                .WithMessage("Slug must be in valid format (lowercase letters, numbers, and hyphens only)");
        }
    }
} 