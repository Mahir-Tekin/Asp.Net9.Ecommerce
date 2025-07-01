using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProductReviews
{
    public record GetProductReviewsQuery : IRequest<Result<ProductReviewListResponse>>
    {
        // Product identifier
        public Guid ProductId { get; init; }
        
        // Sorting
        public ReviewSortBy SortBy { get; init; } = ReviewSortBy.Newest;
        
        // Pagination
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
