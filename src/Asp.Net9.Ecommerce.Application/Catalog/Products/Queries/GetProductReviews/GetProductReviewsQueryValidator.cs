using FluentValidation;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProductReviews
{
    public class GetProductReviewsQueryValidator : AbstractValidator<GetProductReviewsQuery>
    {
        public GetProductReviewsQueryValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("PageNumber must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("PageSize must be between 1 and 100");

            RuleFor(x => x.SortBy)
                .IsInEnum()
                .WithMessage("Invalid sort option");
        }
    }
}
