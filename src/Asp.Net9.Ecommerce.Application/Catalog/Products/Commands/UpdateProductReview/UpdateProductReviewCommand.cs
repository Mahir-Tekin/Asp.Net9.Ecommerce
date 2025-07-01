using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.UpdateProductReview
{
    public record UpdateProductReviewCommand(
        Guid ProductId,
        Guid ReviewId,
        Guid UserId,
        UpdateProductReviewDto Review
    ) : IRequest<Result>;
}
