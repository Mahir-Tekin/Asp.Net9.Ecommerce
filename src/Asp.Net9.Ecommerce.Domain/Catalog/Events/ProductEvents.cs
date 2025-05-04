using Asp.Net9.Ecommerce.Domain.Common;

namespace Asp.Net9.Ecommerce.Domain.Catalog.Events
{
    public record ProductCreatedEvent : IDomainEvent
    {
        public Guid ProductId { get; }
        public string Name { get; }
        public string Slug { get; }
        public decimal BasePrice { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public ProductCreatedEvent(Guid productId, string name, string slug, decimal basePrice)
        {
            ProductId = productId;
            Name = name;
            Slug = slug;
            BasePrice = basePrice;
        }
    }

    public record ProductUpdatedEvent : IDomainEvent
    {
        public Guid ProductId { get; }
        public string Name { get; }
        public string Slug { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public ProductUpdatedEvent(Guid productId, string name, string slug)
        {
            ProductId = productId;
            Name = name;
            Slug = slug;
        }
    }

    public record ProductPriceChangedEvent : IDomainEvent
    {
        public Guid ProductId { get; }
        public decimal OldBasePrice { get; }
        public decimal NewBasePrice { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public ProductPriceChangedEvent(Guid productId, decimal oldBasePrice, decimal newBasePrice)
        {
            ProductId = productId;
            OldBasePrice = oldBasePrice;
            NewBasePrice = newBasePrice;
        }
    }

    public record ProductDeletedEvent : IDomainEvent
    {
        public Guid ProductId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public ProductDeletedEvent(Guid productId)
        {
            ProductId = productId;
        }
    }

    public record VariantLowStockEvent : IDomainEvent
    {
        public Guid VariantId { get; }
        public Guid ProductId { get; }
        public int CurrentStock { get; }
        public int MinStockThreshold { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public VariantLowStockEvent(Guid variantId, Guid productId, int currentStock, int minStockThreshold)
        {
            VariantId = variantId;
            ProductId = productId;
            CurrentStock = currentStock;
            MinStockThreshold = minStockThreshold;
        }
    }
} 