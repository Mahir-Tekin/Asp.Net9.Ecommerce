namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs
{
    public record CategoryVariationTypeInfo
    {
        public Guid VariationTypeId { get; init; }
        public bool IsRequired { get; init; }
    }
} 