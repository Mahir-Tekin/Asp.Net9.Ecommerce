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

    public class ProductListResponse
    {
        public List<ProductListDto> Items { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }

    public class ProductListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public string? MainImage { get; set; }
        
        // Category info
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }

        // Variant summary
        public int VariantCount { get; set; }
        public decimal LowestPrice { get; set; }
        public decimal HighestPrice { get; set; }
        public bool HasStock { get; set; }
        public int TotalStock { get; set; }
        
        // Status
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 