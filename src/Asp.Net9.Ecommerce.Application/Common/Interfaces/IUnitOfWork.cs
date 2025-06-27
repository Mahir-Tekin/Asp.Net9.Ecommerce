using Asp.Net9.Ecommerce.Application.Common.Interfaces.RepositoryInterfaces;

namespace Asp.Net9.Ecommerce.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IVariationTypeRepository VariationTypes { get; }
        IOrderRepository Orders { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}