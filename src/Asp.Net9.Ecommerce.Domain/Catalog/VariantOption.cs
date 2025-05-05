using Asp.Net9.Ecommerce.Domain.Common;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Catalog
{
    public class VariantOption : ValueObject
    {
        public string Value { get; private set; }
        public string DisplayValue { get; private set; }
        public int SortOrder { get; private set; }

        private VariantOption(string value, string displayValue, int sortOrder = 0)
        {
            Value = value.Trim().ToLowerInvariant();
            DisplayValue = displayValue.Trim();
            SortOrder = sortOrder;
        }

        public static Result<VariantOption> Create(string value, string displayValue, int sortOrder = 0)
        {
            var errors = ValidateInputs(value, displayValue);
            if (errors.Any())
                return Result.Failure<VariantOption>(ErrorResponse.ValidationError(errors));

            return Result.Success(new VariantOption(value, displayValue, sortOrder));
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

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return DisplayValue;
            yield return SortOrder;
        }
    }
} 