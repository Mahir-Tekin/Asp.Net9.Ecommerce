using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs
{
    public class CategoryNestedDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public bool IsActive { get; set; }
        public List<CategoryNestedDto> SubCategories { get; set; } = new();
    }
} 