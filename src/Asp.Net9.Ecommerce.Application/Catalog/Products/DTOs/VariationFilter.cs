namespace Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs
{
    /// <summary>
    /// Represents a variation filter for product filtering
    /// Supports both GUID-based and value-based filtering
    /// </summary>
    public class VariationFilter
    {
        /// <summary>
        /// The ID of the variation type (e.g., Color, Size)
        /// Used for GUID-based filtering
        /// </summary>
        public Guid? VariationTypeId { get; set; }

        /// <summary>
        /// The name of the variation type (e.g., "Color", "Size")
        /// Used for value-based filtering
        /// </summary>
        public string? VariationTypeName { get; set; }

        /// <summary>
        /// List of option IDs to filter by (e.g., Red, Blue for Color)
        /// Used for GUID-based filtering
        /// Multiple options within a variation type are OR'ed together
        /// </summary>
        public List<Guid> OptionIds { get; set; } = new();

        /// <summary>
        /// List of option values to filter by (e.g., "Red", "Blue" for Color)
        /// Used for value-based filtering
        /// Multiple options within a variation type are OR'ed together
        /// </summary>
        public List<string> OptionValues { get; set; } = new();
    }
}
