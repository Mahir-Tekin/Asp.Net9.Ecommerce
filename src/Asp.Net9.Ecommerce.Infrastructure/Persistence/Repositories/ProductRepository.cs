using Asp.Net9.Ecommerce.Application.Common.Interfaces.RepositoryInterfaces;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProducts;
using Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs;
using Asp.Net9.Ecommerce.Domain.Catalog;
using Asp.Net9.Ecommerce.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Asp.Net9.Ecommerce.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsBySKUAsync(string sku, CancellationToken cancellationToken = default)
        {
            return await _context.ProductVariants
                .AnyAsync(v => v.SKU == sku && v.Product.DeletedAt == null, cancellationToken);
        }

        public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AnyAsync(p => p.Slug == slug && p.DeletedAt == null, cancellationToken);
        }

        public async Task<Result> AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.Products.AddAsync(product, cancellationToken);
                
                
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ErrorResponse.General(
                    $"Failed to add product to database: {ex.Message}",
                    "PRODUCT_ADD_ERROR"));
            }
        }

        public async Task<Product> GetByIdWithVariantsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Include(p => p.Variants)
                .Include(p => p.VariantTypes)
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null, cancellationToken);
        }

        public async Task<Product> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.SelectedOptions)
                .Include(p => p.VariantTypes)
                    .ThenInclude(vt => vt.Options)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null, cancellationToken);
        }

        public async Task<Product> GetBySlugWithDetailsAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.SelectedOptions)
                .Include(p => p.VariantTypes)
                    .ThenInclude(vt => vt.Options)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.DeletedAt == null, cancellationToken);
        }

        public async Task<Result> UpdateAsync(Product product, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Products.Update(product);
                // Note: SaveChanges is handled by UnitOfWork
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ErrorResponse.General(
                    $"Failed to update product in database: {ex.Message}",
                    "PRODUCT_UPDATE_ERROR"));
            }
        }

        public async Task<(List<Product> Items, int TotalCount)> GetProductListAsync(
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
            CancellationToken cancellationToken = default)
        {
            // Ensure pageNumber and pageSize are at least 1
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            // Start with base query, include images for main image selection and variant options for filtering
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.SelectedOptions)
                        .ThenInclude(so => so.VariationType)
                .Include(p => p.Images)
                .Where(p => p.DeletedAt == null);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowered = searchTerm.Trim().ToLower();
                query = query.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(lowered)) ||
                    p.Variants.Any(v => v.SKU != null && v.SKU.ToLower().Contains(lowered)));
            }

            // Filter by category
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // Filter by price range
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.BasePrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.BasePrice <= maxPrice.Value);
            }

            // Filter by stock availability
            if (hasStock.HasValue)
            {
                if (hasStock.Value)
                {
                    query = query.Where(p => p.Variants.Any(v => v.StockQuantity > 0));
                }
                else
                {
                    query = query.Where(p => p.Variants.All(v => v.StockQuantity <= 0));
                }
            }

            // Filter by active status
            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            // Filter by variation types and options
            if (variationFilters?.Any() == true)
            {
                foreach (var filter in variationFilters)
                {
                    // Support both GUID-based and value-based filtering
                    if (filter.OptionIds?.Any() == true)
                    {
                        // GUID-based filtering: Products must have at least one variant with at least one of the specified option IDs
                        query = query.Where(p => p.Variants.Any(v => 
                            v.SelectedOptions.Any(so => filter.OptionIds.Contains(so.Id))));
                    }
                    else if (filter.OptionValues?.Any() == true && !string.IsNullOrEmpty(filter.VariationTypeName))
                    {
                        // Value-based filtering: Products must have at least one variant with at least one of the specified option values
                        query = query.Where(p => p.Variants.Any(v => 
                            v.SelectedOptions.Any(so => 
                                so.VariationType.Name.ToLower() == filter.VariationTypeName.ToLower() &&
                                filter.OptionValues.Contains(so.Value))));
                    }
                }
            }

            // Get total count before applying pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = sortBy switch
            {
                ProductSortBy.PriceAsc => query.OrderBy(p => p.BasePrice),
                ProductSortBy.PriceDesc => query.OrderByDescending(p => p.BasePrice),
                ProductSortBy.CreatedAtAsc => query.OrderBy(p => p.CreatedAt),
                ProductSortBy.CreatedAtDesc => query.OrderByDescending(p => p.CreatedAt),
                ProductSortBy.RatingAsc => query.OrderBy(p => p.AverageRating),
                ProductSortBy.RatingDesc => query.OrderByDescending(p => p.AverageRating),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            // Apply pagination: skip (pageNumber-1)*pageSize and take pageSize items
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<List<ProductVariant>> GetVariantsByIdsAsync(IEnumerable<Guid> variantIds, CancellationToken cancellationToken = default)
        {
            return await _context.ProductVariants
                .Include(v => v.Product)
                .Where(v => variantIds.Contains(v.Id) && v.Product != null && v.Product.DeletedAt == null)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasUserReviewedProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
        {
            return await _context.ProductReviews
                .AnyAsync(r => r.UserId == userId && r.ProductId == productId && r.DeletedAt == null, cancellationToken);
        }

        public async Task<Product?> GetByIdWithReviewsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AnyAsync(p => p.Id == id && p.DeletedAt == null, cancellationToken);
        }

        public async Task<(IEnumerable<ProductReview> Reviews, int TotalCount, double AverageRating, ReviewRatingSummary RatingSummary)> 
            GetProductReviewsAsync(
                Guid productId,
                ReviewSortBy sortBy,
                int pageNumber,
                int pageSize,
                CancellationToken cancellationToken = default)
        {
            // Base query for reviews without user navigation (to avoid cross-DbContext issues)
            var baseQuery = _context.ProductReviews
                .Where(r => r.ProductId == productId && r.DeletedAt == null);

            // Get total count and rating statistics
            var totalCount = await baseQuery.CountAsync(cancellationToken);
            
            // Calculate average rating and rating summary
            var ratingStats = await baseQuery
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var averageRating = totalCount > 0 
                ? await baseQuery.AverageAsync(r => (double)r.Rating, cancellationToken)
                : 0.0;

            // Build rating summary
            var ratingSummary = new ReviewRatingSummary
            {
                FiveStars = ratingStats.FirstOrDefault(r => r.Rating == 5)?.Count ?? 0,
                FourStars = ratingStats.FirstOrDefault(r => r.Rating == 4)?.Count ?? 0,
                ThreeStars = ratingStats.FirstOrDefault(r => r.Rating == 3)?.Count ?? 0,
                TwoStars = ratingStats.FirstOrDefault(r => r.Rating == 2)?.Count ?? 0,
                OneStar = ratingStats.FirstOrDefault(r => r.Rating == 1)?.Count ?? 0
            };

            // Apply sorting
            var sortedQuery = sortBy switch
            {
                ReviewSortBy.Newest => baseQuery.OrderByDescending(r => r.CreatedAt),
                ReviewSortBy.Oldest => baseQuery.OrderBy(r => r.CreatedAt),
                ReviewSortBy.RatingHigh => baseQuery.OrderByDescending(r => r.Rating).ThenByDescending(r => r.CreatedAt),
                ReviewSortBy.RatingLow => baseQuery.OrderBy(r => r.Rating).ThenByDescending(r => r.CreatedAt),
                ReviewSortBy.MostHelpful => baseQuery.OrderByDescending(r => r.HelpfulVotes).ThenByDescending(r => r.CreatedAt),
                ReviewSortBy.LeastHelpful => baseQuery.OrderBy(r => r.HelpfulVotes).ThenByDescending(r => r.CreatedAt),
                _ => baseQuery.OrderByDescending(r => r.CreatedAt)
            };

            // Apply pagination
            var reviews = await sortedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (reviews, totalCount, averageRating, ratingSummary);
        }

        // Vote-related methods
        public async Task<ReviewVote?> GetVoteByReviewAndUserAsync(Guid reviewId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.ReviewVotes
                .FirstOrDefaultAsync(v => v.ReviewId == reviewId && v.UserId == userId, cancellationToken);
        }

        public async Task AddVoteAsync(ReviewVote vote, CancellationToken cancellationToken = default)
        {
            await _context.ReviewVotes.AddAsync(vote, cancellationToken);
        }

        public async Task UpdateVoteAsync(ReviewVote vote, CancellationToken cancellationToken = default)
        {
            _context.ReviewVotes.Update(vote);
            await Task.CompletedTask;
        }

        public async Task RemoveVoteAsync(ReviewVote vote, CancellationToken cancellationToken = default)
        {
            _context.ReviewVotes.Remove(vote);
            await Task.CompletedTask;
        }

        // Review management methods
        public async Task AddReviewAsync(ProductReview review, CancellationToken cancellationToken = default)
        {
            await _context.ProductReviews.AddAsync(review, cancellationToken);
        }
    }
}