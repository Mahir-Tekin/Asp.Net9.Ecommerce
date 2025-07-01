using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Commands.DeleteProductReview;

public sealed record DeleteProductReviewCommand(
    Guid ProductId,
    Guid ReviewId,
    Guid UserId
) : IRequest<Result>;
