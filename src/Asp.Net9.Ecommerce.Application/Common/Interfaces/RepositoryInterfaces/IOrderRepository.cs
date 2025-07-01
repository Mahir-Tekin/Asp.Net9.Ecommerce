using Asp.Net9.Ecommerce.Domain.Orders;
using System.Linq;

namespace Asp.Net9.Ecommerce.Application.Common.Interfaces.RepositoryInterfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Order order, CancellationToken cancellationToken = default);
        Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
        IQueryable<Order> GetQueryable();
        Task<bool> HasUserPurchasedAndReceivedProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
    }
}
