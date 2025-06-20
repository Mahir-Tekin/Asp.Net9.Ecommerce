using Asp.Net9.Ecommerce.Domain.Common;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Catalog
{
    public class CategoryVariationType : ValueObject
    {
        public Guid VariationTypeId { get; private set; }
        public bool IsRequired { get; private set; }

        private CategoryVariationType(Guid variationTypeId, bool isRequired)
        {
            VariationTypeId = variationTypeId;
            IsRequired = isRequired;
        }

        public static Result<CategoryVariationType> Create(Guid variationTypeId, bool isRequired)
        {
            if (variationTypeId == Guid.Empty)
                return Result.Failure<CategoryVariationType>(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("VariationTypeId", "Variation type ID is required") }));

            return Result.Success(new CategoryVariationType(variationTypeId, isRequired));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return VariationTypeId;
            yield return IsRequired;
        }
    }
} 