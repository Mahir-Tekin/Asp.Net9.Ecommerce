namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs
{
    public class VariationTypeResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<VariationOptionResponse> Options { get; set; } = new();
    }

    public class VariationOptionResponse
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public string DisplayValue { get; set; }
        public int SortOrder { get; set; }
    }
} 