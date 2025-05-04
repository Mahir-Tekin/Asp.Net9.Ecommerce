using Asp.Net9.Ecommerce.Domain.Common;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Catalog
{
    public class ProductImage : ValueObject
    {
        public string Url { get; private set; }
        public string? AltText { get; private set; }
        public bool IsMain { get; private set; }

        private ProductImage(string url, string? altText, bool isMain)
        {
            Url = url;
            AltText = altText;
            IsMain = isMain;
        }

        public static Result<ProductImage> Create(string url, string? altText = null, bool isMain = false)
        {
            if (string.IsNullOrWhiteSpace(url))
                return Result.Failure<ProductImage>(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Url", "Image URL is required") }));

            return Result.Success(new ProductImage(url, altText, isMain));
        }

        public void SetMain() => IsMain = true;
        public void UnsetMain() => IsMain = false;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Url;
            if (AltText != null) yield return AltText;
            yield return IsMain;
        }
    }
} 