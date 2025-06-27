using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs
{
    /// <summary>
    /// Response containing available filters for a category
    /// </summary>
    public class CategoryFiltersResponse
    {
        /// <summary>
        /// The category ID
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// The category name
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Available variation types and their options for filtering
        /// </summary>
        public List<VariationTypeDto> VariationTypes { get; set; } = new();
    }
}
