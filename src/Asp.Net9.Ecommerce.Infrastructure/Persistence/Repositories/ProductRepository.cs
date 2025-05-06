using Asp.Net9.Ecommerce.Application.Common.Interfaces.RepositoryInterfaces;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProducts;
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

        public async Task<Result> AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.Products.AddAsync(product, cancellationToken);
                
                // Note: We don't call SaveChanges here because that's handled by the UnitOfWork
                // This keeps the unit of work pattern intact while still providing error handling
                
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
                .Include(p => p.VariantTypes)
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null, cancellationToken);
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
            ProductSortBy sortBy = ProductSortBy.CreatedAtDesc,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            // Start with base query
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .Where(p => p.DeletedAt == null);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(p => 
                    p.Name.ToLower().Contains(searchTerm) || 
                    p.Description.ToLower().Contains(searchTerm) ||
                    p.Variants.Any(v => v.SKU.ToLower().Contains(searchTerm)));
            }

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.BasePrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.BasePrice <= maxPrice.Value);

            if (hasStock.HasValue)
            {
                query = hasStock.Value
                    ? query.Where(p => p.Variants.Any(v => !v.TrackInventory || v.StockQuantity > 0))
                    : query.Where(p => p.Variants.All(v => v.TrackInventory && v.StockQuantity == 0));
            }

            if (isActive.HasValue)
                query = query.Where(p => p.IsActive == isActive.Value);

            // Get total count before applying pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = sortBy switch
            {
                ProductSortBy.NameAsc => query.OrderBy(p => p.Name),
                ProductSortBy.NameDesc => query.OrderByDescending(p => p.Name),
                ProductSortBy.PriceAsc => query.OrderBy(p => p.BasePrice),
                ProductSortBy.PriceDesc => query.OrderByDescending(p => p.BasePrice),
                ProductSortBy.CreatedAtAsc => query.OrderBy(p => p.CreatedAt),
                ProductSortBy.CreatedAtDesc => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            // Apply pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
} 