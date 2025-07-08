

using Asp.Net9.Ecommerce.Application.Catalog.VariationTypes.DTOs;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs
{
    /// <summary>
    /// Represents a variation type associated with a category
    /// </summary>
    public class CategoryVariationTypeDto
    {
        /// <summary>
        /// The unique identifier of the variation type
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid VariationTypeId { get; set; }

        /// <summary>
        /// Whether this variation type is required when creating products in this category
        /// </summary>
        /// <example>true</example>
        public bool IsRequired { get; set; }

        /// <summary>
        /// The name of the variation type
        /// </summary>
        /// <example>Color</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The user-friendly display name of the variation type
        /// </summary>
        /// <example>Shoe Size</example>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// The list of options for this variation type
        /// </summary>
        public List<VariantOptionDto> Options { get; set; } = new();
    }
}