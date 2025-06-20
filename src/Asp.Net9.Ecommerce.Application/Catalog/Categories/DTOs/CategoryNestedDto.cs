using Asp.Net9.Ecommerce.Shared.Results;
using System.ComponentModel.DataAnnotations;

namespace Asp.Net9.Ecommerce.Application.Catalog.Categories.DTOs
{
    /// <summary>
    /// Represents a category in a nested tree structure
    /// </summary>
    public class CategoryNestedDto
    {
        /// <summary>
        /// The unique identifier of the category
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the category
        /// </summary>
        /// <example>Electronics</example>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The description of the category
        /// </summary>
        /// <example>All electronic devices and accessories</example>
        public string Description { get; set; }

        /// <summary>
        /// The URL-friendly slug of the category
        /// </summary>
        /// <example>electronics</example>
        [Required]
        public string Slug { get; set; }

        /// <summary>
        /// Whether the category is active and visible to customers
        /// </summary>
        /// <example>true</example>
        public bool IsActive { get; set; }

        /// <summary>
        /// The ID of the parent category, if any
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid? ParentCategoryId { get; set; }

        /// <summary>
        /// The subcategories of this category
        /// </summary>
        /// <example>
        /// [
        ///   {
        ///     "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///     "name": "Laptops",
        ///     "description": "All laptop computers",
        ///     "slug": "laptops",
        ///     "isActive": true,
        ///     "parentCategoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///     "subCategories": [],
        ///     "variationTypes": []
        ///   }
        /// ]
        /// </example>
        public List<CategoryNestedDto> SubCategories { get; set; } = new();

        /// <summary>
        /// The variation types associated with this category
        /// </summary>
        public List<CategoryVariationTypeDto> VariationTypes { get; set; } = new();
    }
} 