using Asp.Net9.Ecommerce.Application.Common.Interfaces.RepositoryInterfaces;
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
            // Include both subcategories, parent category, and variation types for complete hierarchy
            return await _context.Categories
                .Include(c => c.SubCategories)
                .Include(c => c.ParentCategory)
                .Include(c => c.VariationTypes)
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null, cancellationToken);
        }

        public async Task<Category> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            // Include both subcategories, parent category, and variation types for complete hierarchy
            return await _context.Categories
                .Include(c => c.SubCategories)
                .Include(c => c.ParentCategory)
                .Include(c => c.VariationTypes)
                .FirstOrDefaultAsync(c => c.Slug == slug && c.DeletedAt == null, cancellationToken);
        }

        public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AnyAsync(c => c.Slug == slug && c.DeletedAt == null, cancellationToken);
        }

        public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AnyAsync(c => c.Id == id && c.DeletedAt == null, cancellationToken);
        }

        public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            // Include both subcategories and parent categories for complete hierarchy
            return await _context.Categories
                .Include(c => c.SubCategories)
                .Include(c => c.ParentCategory)
                .Where(c => c.DeletedAt == null)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Category>> GetCategoryTreeAsync(CancellationToken cancellationToken = default)
        {
            // Get all active categories with their relationships
            var allCategories = await _context.Categories
                .Include(c => c.SubCategories)
                .Include(c => c.ParentCategory)
                .Where(c => c.DeletedAt == null && c.IsActive)
                .ToListAsync(cancellationToken);

            // Build the tree structure
            var rootCategories = allCategories
                .Where(c => c.ParentCategoryId == null)
                .ToList();

            return rootCategories;
        }

        public void Add(Category category)
        {
            // Add the aggregate root
            _context.Categories.Add(category);
        }

        public void Update(Category category)
        {
            // Update the aggregate root
            // EF Core will track changes to subcategories automatically
            _context.Categories.Update(category);
        }

        public void Delete(Category category)
        {
            // Soft delete the aggregate root
            category.SetDeleted();
            _context.Categories.Update(category);
        }

        public async Task<Category> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Include(c => c.SubCategories)
                .Include(c => c.VariationTypes)
                    .ThenInclude(vt => vt.Options)
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null, cancellationToken);
        }
    }
}