namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs
{
    public class UpdateCategoryRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public bool IsActive { get; set; }
        public List<CategoryVariationTypeRequest> VariationTypes { get; set; } = new();
    }
} 