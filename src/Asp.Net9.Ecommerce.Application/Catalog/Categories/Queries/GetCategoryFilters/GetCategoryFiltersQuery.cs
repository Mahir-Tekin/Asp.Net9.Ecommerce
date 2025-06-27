using Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.Queries.GetCategoryFilters
{
    /// <summary>
    /// Query to get available filters for a category
    /// </summary>
    public record GetCategoryFiltersQuery : IRequest<Result<CategoryFiltersResponse>>
    {
        /// <summary>
        /// The category ID to get filters for
        /// </summary>
        public Guid CategoryId { get; init; }
    }
}
