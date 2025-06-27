namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs
{
    public class VariantOptionDto
    {
        public Guid Id { get; set; }
        public string Value { get; set; } = string.Empty;
        public string DisplayValue { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
