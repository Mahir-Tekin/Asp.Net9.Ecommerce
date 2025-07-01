using FluentValidation;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.UpdateProductReview
{
    public class UpdateProductReviewCommandValidator : AbstractValidator<UpdateProductReviewCommand>
    {
        public UpdateProductReviewCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID is required");

            RuleFor(x => x.ReviewId)
                .NotEmpty().WithMessage("Review ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.Review.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

            RuleFor(x => x.Review.Title)
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Review.Title));

            RuleFor(x => x.Review.Comment)
                .MaximumLength(2000).WithMessage("Comment cannot exceed 2000 characters")
                .When(x => !string.IsNullOrEmpty(x.Review.Comment));

            // At least title or comment must be provided
            RuleFor(x => x.Review)
                .Must(review => !string.IsNullOrWhiteSpace(review.Title) || !string.IsNullOrWhiteSpace(review.Comment))
                .WithMessage("Either title or comment must be provided");
        }
    }
}
