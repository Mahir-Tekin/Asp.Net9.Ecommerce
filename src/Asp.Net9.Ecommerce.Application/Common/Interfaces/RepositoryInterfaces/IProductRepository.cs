using Asp.Net9.Ecommerce.Domain.Catalog;

namespace Asp.Net9.Ecommerce.Application.Common.Interfaces.RepositoryInterfaces
{
    public interface IProductRepository
    {
        // For creating a product, we need to:
        // 1. Check if SKU exists (to ensure uniqueness)
        // 2. Add the product
        // 3. Save changes
        Task<bool> ExistsBySKUAsync(string sku, CancellationToken cancellationToken = default);
        void Add(Product product);
    }
} 