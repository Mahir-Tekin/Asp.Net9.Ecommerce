namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs
{
    public class CreateVariationTypeRequest
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<CreateVariationOptionRequest> Options { get; set; } = new();
    }

    public class CreateVariationOptionRequest
    {
        public string Value { get; set; }
        public string DisplayValue { get; set; }
        public int SortOrder { get; set; }
    }
} 