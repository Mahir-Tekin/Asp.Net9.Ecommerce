using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;
using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProducts
{
    public record GetProductsQuery : IRequest<Result<ProductListResponse>>
    {
        // Search
        public string? SearchTerm { get; init; }
        
        // Filtering
        public Guid? CategoryId { get; init; }
        public decimal? MinPrice { get; init; }
        public decimal? MaxPrice { get; init; }
        public bool? HasStock { get; init; }
        public bool? IsActive { get; init; }
        
        // Sorting
        public ProductSortBy SortBy { get; init; } = ProductSortBy.CreatedAtDesc;
        
        // Pagination
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }

    public enum ProductSortBy
    {
        NameAsc,
        NameDesc,
        PriceAsc,
        PriceDesc,
        CreatedAtAsc,
        CreatedAtDesc
    }
} 