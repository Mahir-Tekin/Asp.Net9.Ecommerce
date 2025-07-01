using FluentValidation;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands
{
    public class CreateProductReviewCommandValidator : AbstractValidator<CreateProductReviewCommand>
    {
        public CreateProductReviewCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.Dto)
                .NotNull().WithMessage("Review data is required")
                .ChildRules(dto =>
                {
                    dto.RuleFor(x => x.ProductId)
                        .NotEmpty().WithMessage("Product ID is required");

                    dto.RuleFor(x => x.Rating)
                        .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

                    dto.RuleFor(x => x.Title)
                        .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
                        .When(x => !string.IsNullOrWhiteSpace(x.Title));

                    dto.RuleFor(x => x.Comment)
                        .MaximumLength(2000).WithMessage("Comment cannot exceed 2000 characters")
                        .When(x => !string.IsNullOrWhiteSpace(x.Comment));

                    // At least title or comment should be provided
                    dto.RuleFor(x => x)
                        .Must(x => !string.IsNullOrWhiteSpace(x.Title) || !string.IsNullOrWhiteSpace(x.Comment))
                        .WithMessage("Either title or comment must be provided")
                        .WithName("Review");
                });
        }
    }
}
