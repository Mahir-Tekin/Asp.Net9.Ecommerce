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
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null, cancellationToken);
        }

        public async Task<Category> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
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
            return await _context.Categories
                .Where(c => c.DeletedAt == null)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Category>> GetCategoryTreeAsync(CancellationToken cancellationToken = default)
        {
            // Get all active categories in a single query
            var allCategories = await _context.Categories
                .Where(c => c.DeletedAt == null && c.IsActive)
                .ToListAsync(cancellationToken);

            // Organize categories by their parent ID for efficient lookup
            var categoryLookup = allCategories
                .ToDictionary(c => c.Id, c => c);

            // Build the tree structure
            var rootCategories = new List<Category>();
            
            // First, identify root categories
            foreach (var category in allCategories)
            {
                if (category.ParentCategoryId == null)
                {
                    rootCategories.Add(category);
                }
            }
            
            // Then, build the hierarchy by attaching children to their parents
            foreach (var category in allCategories)
            {
                if (category.ParentCategoryId != null && 
                    categoryLookup.TryGetValue(category.ParentCategoryId.Value, out var parentCategory))
                {
                    parentCategory.AddSubCategory(category);
                }
            }

            return rootCategories;
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