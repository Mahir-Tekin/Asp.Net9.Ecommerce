using Asp.Net9.Ecommerce.Domain.Common;

namespace Asp.Net9.Ecommerce.Domain.Common
{
    public interface IAggregateRoot
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void AddDomainEvent(IDomainEvent @event);
        void RemoveDomainEvent(IDomainEvent @event);
        void ClearDomainEvents();
    }
} 