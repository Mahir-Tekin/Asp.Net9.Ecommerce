using Asp.Net9.Ecommerce.Application.Common.Interfaces.RepositoryInterfaces;
using Asp.Net9.Ecommerce.Domain.Catalog;
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

        public void Add(Product product)
        {
            _context.Products.Add(product);
        }
    }
} 