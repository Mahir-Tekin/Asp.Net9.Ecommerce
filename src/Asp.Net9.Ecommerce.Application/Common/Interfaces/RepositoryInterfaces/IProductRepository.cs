using Asp.Net9.Ecommerce.Domain.Catalog;
using Asp.Net9.Ecommerce.Shared.Results;
using Asp.Net9.Ecommerce.Application.Catalog.Products.Queries.GetProducts;

namespace Asp.Net9.Ecommerce.Application.Common.Interfaces.RepositoryInterfaces
{
    public interface IProductRepository
    {
        // For creating a product, we need to:
        // 1. Check if SKU exists (to ensure uniqueness)
        // 2. Add the product
        // 3. Save changes
        Task<bool> ExistsBySKUAsync(string sku, CancellationToken cancellationToken = default);
        Task<Result> AddAsync(Product product, CancellationToken cancellationToken = default);
        Task<Product> GetByIdWithVariantsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Product> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(Product product, CancellationToken cancellationToken = default);
        
        // New method for filtered and paginated queries
        Task<(List<Product> Items, int TotalCount)> GetProductListAsync(
            string? searchTerm = null,
            Guid? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? hasStock = null,
            bool? isActive = null,
            ProductSortBy sortBy = ProductSortBy.CreatedAtDesc,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);
    }
} 