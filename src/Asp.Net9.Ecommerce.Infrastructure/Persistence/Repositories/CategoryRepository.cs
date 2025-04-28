using Asp.Net9.Ecommerce.Application.Common.Interfaces;
using Asp.Net9.Ecommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Asp.Net9.Ecommerce.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Category> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
        }

        public async Task<Category> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Slug == slug && !c.IsDeleted, cancellationToken);
        }

        public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AnyAsync(c => c.Slug == slug && !c.IsDeleted, cancellationToken);
        }

        public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AnyAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
        }

        public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Where(c => !c.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Category>> GetCategoryTreeAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Where(c => !c.IsDeleted && c.ParentCategoryId == null) // Get root categories
                .ToListAsync(cancellationToken);
        }

        public void Add(Category category)
        {
            _context.Categories.Add(category);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }

        public void Delete(Category category)
        {
            // Note: This is a soft delete
            category.SetDeleted();
            _context.Categories.Update(category);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
} 