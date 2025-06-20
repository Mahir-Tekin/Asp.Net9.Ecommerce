namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs
{
    public class UpdateVariationTypeRequest
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
        public List<UpdateVariationOptionRequest> Options { get; set; } = new();
    }

    public class UpdateVariationOptionRequest
    {
        public string Value { get; set; }
        public string DisplayValue { get; set; }
        public int SortOrder { get; set; }
    }
} 