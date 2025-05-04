using Asp.Net9.Ecommerce.Domain.Common;

namespace Asp.Net9.Ecommerce.Domain.Catalog.Events
{
    public record CategoryCreatedEvent : IDomainEvent
    {
        public Guid CategoryId { get; }
        public string Name { get; }
        public string Slug { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public CategoryCreatedEvent(Guid categoryId, string name, string slug)
        {
            CategoryId = categoryId;
            Name = name;
            Slug = slug;
        }
    }

    public record CategoryUpdatedEvent : IDomainEvent
    {
        public Guid CategoryId { get; }
        public string Name { get; }
        public string Slug { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public CategoryUpdatedEvent(Guid categoryId, string name, string slug)
        {
            CategoryId = categoryId;
            Name = name;
            Slug = slug;
        }
    }

    public record CategoryDeletedEvent : IDomainEvent
    {
        public Guid CategoryId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public CategoryDeletedEvent(Guid categoryId)
        {
            CategoryId = categoryId;
        }
    }
} 