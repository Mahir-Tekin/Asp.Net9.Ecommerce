using Asp.Net9.Ecommerce.Domain.Common;

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

        public static VariantOption Create(string value, string displayValue, int sortOrder = 0)
        {
            return new VariantOption(value, displayValue, sortOrder);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return DisplayValue;
            yield return SortOrder;
        }
    }
} 