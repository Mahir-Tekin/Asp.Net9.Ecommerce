using System.ComponentModel.DataAnnotations;

namespace Asp.Net9.Ecommerce.Application.Catalog.Products.DTOs
{
    public class UpdateProductReviewDto
    {
        /// <summary>
        /// Rating from 1 to 5 stars
        /// </summary>
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        /// <summary>
        /// Optional review title
        /// </summary>
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        /// <summary>
        /// Optional review comment
        /// </summary>
        [MaxLength(2000, ErrorMessage = "Comment cannot exceed 2000 characters")]
        public string? Comment { get; set; }
    }
}
