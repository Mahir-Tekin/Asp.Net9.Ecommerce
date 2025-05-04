using System.Collections.Generic;

namespace Asp.Net9.Ecommerce.Domain.Common
{
    public abstract class AggregateRoot : BaseEntity, IAggregateRoot
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent @event)
        {
            _domainEvents.Add(@event);
        }

        public void RemoveDomainEvent(IDomainEvent @event)
        {
            _domainEvents.Remove(@event);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
} 