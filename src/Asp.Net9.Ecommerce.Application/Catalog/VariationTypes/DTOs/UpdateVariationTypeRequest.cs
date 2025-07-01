namespace Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs
{
    public class UpdateVariationTypeRequest
    {
        public required string Name { get; set; }
        public required string DisplayName { get; set; }
        public bool IsActive { get; set; }
        public List<UpdateVariationOptionRequest> Options { get; set; } = new();
    }

    public class UpdateVariationOptionRequest
    {
        public Guid? Id { get; set; } // Null for new options, set for existing options to update
        public required string Value { get; set; }
        public required string DisplayValue { get; set; }
        public int SortOrder { get; set; }
    }
} 