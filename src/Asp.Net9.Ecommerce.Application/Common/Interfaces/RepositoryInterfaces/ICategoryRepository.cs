using Asp.Net9.Ecommerce.Domain.Catalog;

namespace Asp.Net9.Ecommerce.Application.Common.Interfaces.RepositoryInterfaces
{
    public interface ICategoryRepository
    {
        Task<Category> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Category> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Category>> GetCategoryTreeAsync(CancellationToken cancellationToken = default);
        Task<Category> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
        void Add(Category category);
        void Update(Category category);
        void Delete(Category category);
    }
} 