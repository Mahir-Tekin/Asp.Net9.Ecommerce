namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public List<CategoryVariationTypeRequest>? VariationTypes { get; set; } = new();
    }

    public class CategoryVariationTypeRequest
    {
        public Guid VariationTypeId { get; set; }
        public bool IsRequired { get; set; }
    }
} 