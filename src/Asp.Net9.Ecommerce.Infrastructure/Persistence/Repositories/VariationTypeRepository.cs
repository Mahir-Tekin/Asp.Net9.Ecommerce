using Asp.Net9.Ecommerce.Application.Common.Interfaces.RepositoryInterfaces;
using Asp.Net9.Ecommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Asp.Net9.Ecommerce.Infrastructure.Persistence.Repositories
{
    public class VariationTypeRepository : IVariationTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public VariationTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<VariationType> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.VariationTypes
                .FirstOrDefaultAsync(vt => vt.Id == id && vt.DeletedAt == null, cancellationToken);
        }

        public async Task<VariationType> GetByIdWithOptionsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.VariationTypes
                .Include(vt => vt.Options)
                .FirstOrDefaultAsync(vt => vt.Id == id && vt.DeletedAt == null, cancellationToken);
        }

        public async Task<VariationType> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.VariationTypes
                .FirstOrDefaultAsync(vt => vt.Name == name.Trim().ToLowerInvariant() && vt.DeletedAt == null, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.VariationTypes
                .AnyAsync(vt => vt.Name == name.Trim().ToLowerInvariant() && vt.DeletedAt == null, cancellationToken);
        }

        public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.VariationTypes
                .AnyAsync(vt => vt.Id == id && vt.DeletedAt == null, cancellationToken);
        }

        public async Task<IReadOnlyList<VariationType>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.VariationTypes
                .Include(vt => vt.Options)
                .Where(vt => vt.DeletedAt == null)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<VariationType>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            return await _context.VariationTypes
                .Include(vt => vt.Options)
                .Where(vt => ids.Contains(vt.Id) && vt.DeletedAt == null)
                .ToListAsync(cancellationToken);
        }

        public void Add(VariationType variationType)
        {
            _context.VariationTypes.Add(variationType);
        }

        public void Update(VariationType variationType)
        {
            _context.VariationTypes.Update(variationType);
        }

        public void Delete(VariationType variationType)
        {
            variationType.SetDeleted();
            _context.VariationTypes.Update(variationType);
        }

        public void AddOption(VariantOption option)
        {
            // Check if the option is already being tracked
            var entry = _context.Entry(option);
            if (entry.State == EntityState.Detached)
            {
                _context.Set<VariantOption>().Add(option);
            }
        }
    }
}