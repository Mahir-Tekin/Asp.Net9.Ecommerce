namespace Asp.Net9.Ecommerce.Domain.Common
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
} 