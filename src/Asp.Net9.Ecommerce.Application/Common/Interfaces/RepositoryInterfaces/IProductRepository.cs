using Asp.Net9.Ecommerce.Domain.Catalog;
using Asp.Net9.Ecommerce.Shared.Results;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProducts;
using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;

namespace Asp.Net9.Ecommerce.Application.Common.Interfaces.RepositoryInterfaces
{
    public interface IProductRepository
    {
        // For creating a product, we need to:
        // 1. Check if SKU exists (to ensure uniqueness)
        // 2. Add the product
        // 3. Save changes
        Task<bool> ExistsBySKUAsync(string sku, CancellationToken cancellationToken = default);
        Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<Result> AddAsync(Product product, CancellationToken cancellationToken = default);
        Task<Product> GetByIdWithVariantsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Product> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Product> GetBySlugWithDetailsAsync(string slug, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(Product product, CancellationToken cancellationToken = default);
        
        // New method for filtered and paginated queries
        Task<(List<Product> Items, int TotalCount)> GetProductListAsync(
            string? searchTerm = null,
            Guid? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? hasStock = null,
            bool? isActive = null,
            List<VariationFilter>? variationFilters = null,
            ProductSortBy sortBy = ProductSortBy.CreatedAtDesc,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);

        Task<List<ProductVariant>> GetVariantsByIdsAsync(IEnumerable<Guid> variantIds, CancellationToken cancellationToken = default);
        Task<bool> HasUserReviewedProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
        Task<Product?> GetByIdWithReviewsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        
        // Review-related methods
        Task AddReviewAsync(ProductReview review, CancellationToken cancellationToken = default);
        Task<(IEnumerable<ProductReview> Reviews, int TotalCount, double AverageRating, ReviewRatingSummary RatingSummary)> 
            GetProductReviewsAsync(
                Guid productId,
                ReviewSortBy sortBy,
                int pageNumber,
                int pageSize,
                CancellationToken cancellationToken = default);

        // Vote-related methods
        Task<ReviewVote?> GetVoteByReviewAndUserAsync(Guid reviewId, Guid userId, CancellationToken cancellationToken = default);
        Task AddVoteAsync(ReviewVote vote, CancellationToken cancellationToken = default);
        Task UpdateVoteAsync(ReviewVote vote, CancellationToken cancellationToken = default);
        Task RemoveVoteAsync(ReviewVote vote, CancellationToken cancellationToken = default);
    }
}