using FluentValidation;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.VoteOnProductReview
{
    public class VoteOnProductReviewCommandValidator : AbstractValidator<VoteOnProductReviewCommand>
    {
        public VoteOnProductReviewCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID is required");

            RuleFor(x => x.ReviewId)
                .NotEmpty().WithMessage("Review ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.VoteType)
                .IsInEnum().WithMessage("Vote type must be either Helpful or Unhelpful");
        }
    }
}
