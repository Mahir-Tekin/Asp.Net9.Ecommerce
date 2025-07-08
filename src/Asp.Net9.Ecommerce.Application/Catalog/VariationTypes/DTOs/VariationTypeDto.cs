namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs
{
    public class VariationTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public List<VariantOptionDto> Options { get; set; } = new();
    }
}
