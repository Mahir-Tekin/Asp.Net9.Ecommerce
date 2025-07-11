using Asp.Net9.Ecommerce.Domain.Catalog;

namespace Asp.Net9.Ecommerce.Application.Common.Interfaces.RepositoryInterfaces
{
    public interface IVariationTypeRepository
    {
        Task<VariationType> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<VariationType> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<VariationType> GetByIdWithOptionsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<VariationType>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<VariationType>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
        void Add(VariationType variationType);
        void Update(VariationType variationType);
        void Delete(VariationType variationType);
        void AddOption(VariantOption option);
    }
}