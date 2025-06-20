using Asp.Net9.Ecommerce.Domain.Common;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Catalog
{
    public class VariantOption : BaseEntity
    {
        public string Value { get; private set; }
        public string DisplayValue { get; private set; }
        public int SortOrder { get; private set; }

        public Guid VariationTypeId { get; internal set; }
        public VariationType VariationType { get; private set; }

        private VariantOption(string value, string displayValue, int sortOrder = 0)
        {
            Value = value.Trim().ToLowerInvariant();
            DisplayValue = displayValue.Trim();
            SortOrder = sortOrder;
        }

        protected VariantOption() { } // For EF Core

        public static Result<VariantOption> Create(string value, string displayValue, int sortOrder = 0, Guid? variationTypeId = null)
        {
            var errors = ValidateInputs(value, displayValue);
            if (errors.Any())
                return Result.Failure<VariantOption>(ErrorResponse.ValidationError(errors));

            var option = new VariantOption(value, displayValue, sortOrder);
            if (variationTypeId.HasValue)
                option.VariationTypeId = variationTypeId.Value;

            return Result.Success(option);
        }

        private static List<ValidationError> ValidateInputs(string value, string displayValue)
        {
            var errors = new List<ValidationError>();

            if (string.IsNullOrWhiteSpace(value))
                errors.Add(new ValidationError("Value", "Value is required"));
            else if (value.Length > 50)
                errors.Add(new ValidationError("Value", "Value cannot exceed 50 characters"));

            if (string.IsNullOrWhiteSpace(displayValue))
                errors.Add(new ValidationError("DisplayValue", "Display value is required"));
            else if (displayValue.Length > 100)
                errors.Add(new ValidationError("DisplayValue", "Display value cannot exceed 100 characters"));

            return errors;
        }
    }
}