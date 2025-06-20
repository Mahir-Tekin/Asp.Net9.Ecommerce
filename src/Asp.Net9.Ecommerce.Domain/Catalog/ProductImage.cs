using Asp.Net9.Ecommerce.Domain.Common;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Catalog
{
    public class ProductImage : BaseEntity
    {
        public string Url { get; private set; }
        public string? AltText { get; private set; }
        public bool IsMain { get; private set; }

        // Foreign key and navigation property
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; }

        private ProductImage() { } // For EF Core

        private ProductImage(string url, string? altText, bool isMain, Guid productId)
        {
            Url = url;
            AltText = altText;
            IsMain = isMain;
            ProductId = productId;
        }

        public static Result<ProductImage> Create(string url, string? altText = null, bool isMain = false, Guid? productId = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                return Result.Failure<ProductImage>(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Url", "Image URL is required") }));

            // productId can be set after construction if not provided
            return Result.Success(new ProductImage(url, altText, isMain, productId ?? Guid.Empty));
        }

        public void SetMain() => IsMain = true;
        public void UnsetMain() => IsMain = false;
    }
} 