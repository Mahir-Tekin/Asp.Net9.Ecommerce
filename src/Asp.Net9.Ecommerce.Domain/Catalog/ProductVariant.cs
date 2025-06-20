using Asp.Net9.Ecommerce.Domain.Common;
using Asp.Net9.Ecommerce.Domain.Catalog.Events;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Catalog
{
    public class ProductVariant : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public string? SKU { get; private set; }
        public string? Name { get; private set; }

        // Pricing - each variant can have its own price or inherit from product
        private decimal? _price;
        public decimal Price => _price ?? Product!.BasePrice;
        public decimal? OldPrice { get; private set; }

        // Inventory
        private int _stockQuantity;
        public int StockQuantity => _stockQuantity;
        public int MinStockThreshold { get; private set; }
        public bool IsActive { get; private set; }
        public bool TrackInventory { get; private set; }

        // Variations - stores selected VariantOption IDs per VariationType ID
        private readonly Dictionary<Guid, Guid> _variantOptions = new();

        // Navigation property for many-to-many
        public ICollection<VariantOption> SelectedOptions { get; private set; } = new List<VariantOption>();

        // Navigation property
        public Product? Product { get; private set; }

        protected ProductVariant() { } // For EF Core

        public static Result<ProductVariant> Create(
            Guid productId,
            string sku,
            string name,
            decimal? price,
            IReadOnlyCollection<VariationType> variantTypes,
            IDictionary<Guid, Guid> selectedOptions,
            int stockQuantity = 0,
            int minStockThreshold = 10,
            bool trackInventory = true)
        {
            var errors = ValidateInputs(sku, name, price, minStockThreshold);
            if (errors.Any())
                return Result.Failure<ProductVariant>(ErrorResponse.ValidationError(errors));

            // Validate that all required variant types are present
            var missingTypes = variantTypes.Select(vt => vt.Id).Except(selectedOptions.Keys).ToList();
            if (missingTypes.Any())
            {
                return Result.Failure<ProductVariant>(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Variant",
                        $"Missing options for variant types: {string.Join(", ", missingTypes)}") }));
            }

            // Validate that each selected option exists and belongs to the correct type
            var selectedOptionEntities = new List<VariantOption>();
            foreach (var kvp in selectedOptions)
            {
                var type = variantTypes.FirstOrDefault(vt => vt.Id == kvp.Key);
                if (type == null)
                {
                    return Result.Failure<ProductVariant>(ErrorResponse.ValidationError(
                        new List<ValidationError> { new("Variant",
                            $"Variant type {kvp.Key} is not defined for this product") }));
                }
                var option = type.Options.FirstOrDefault(o => o.Id == kvp.Value);
                if (option == null)
                {
                    return Result.Failure<ProductVariant>(ErrorResponse.ValidationError(
                        new List<ValidationError> { new("Variant",
                            $"Option {kvp.Value} is not valid for variant type {type.Name}") }));
                }
                selectedOptionEntities.Add(option);
            }

            var variant = new ProductVariant
            {
                ProductId = productId,
                SKU = sku.Trim().ToUpperInvariant(),
                Name = name.Trim(),
                _price = price,
                _stockQuantity = stockQuantity,
                MinStockThreshold = minStockThreshold,
                IsActive = true,
                TrackInventory = trackInventory,
                SelectedOptions = selectedOptionEntities
            };

            // Set the _variantOptions dictionary
            foreach (var kvp in selectedOptions)
            {
                variant._variantOptions[kvp.Key] = kvp.Value;
            }

            return Result.Success(variant);
        }


        public Result AddOption(Guid variationTypeId, Guid optionId, VariantOption? option = null)
        {
            if (variationTypeId == Guid.Empty)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("VariationTypeId", "Variation type ID is required") }));
            if (optionId == Guid.Empty)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("OptionId", "Option ID is required") }));

            _variantOptions[variationTypeId] = optionId;
            if (option != null && !SelectedOptions.Any(o => o.Id == option.Id))
                SelectedOptions.Add(option);
            return Result.Success();
        }


        public Result RemoveOption(Guid variationTypeId)
        {
            if (!_variantOptions.ContainsKey(variationTypeId))
                return Result.NotFound($"Variation type {variationTypeId} not found");
            _variantOptions.Remove(variationTypeId);
            var toRemove = SelectedOptions.FirstOrDefault(o => o.VariationTypeId == variationTypeId);
            if (toRemove != null)
                SelectedOptions.Remove(toRemove);
            return Result.Success();
        }


        public bool HasOption(Guid variationTypeId)
        {
            return _variantOptions.ContainsKey(variationTypeId);
        }


        public Guid? GetOptionId(Guid variationTypeId)
        {
            return _variantOptions.TryGetValue(variationTypeId, out var value) ? value : null;
        }


        public IReadOnlyDictionary<Guid, Guid> GetVariantOptions()
        {
            return new Dictionary<Guid, Guid>(_variantOptions);
        }

        public Result UpdateStock(int quantity)
        {
            if (!TrackInventory)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Stock", "This variant does not track inventory") }));

            if (quantity < 0)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Stock", "Stock quantity cannot be negative") }));

            _stockQuantity = quantity;
            return Result.Success();
        }

        public Result UpdatePrice(decimal? newPrice)
        {
            if (newPrice.HasValue && newPrice.Value <= 0)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Price", "Price must be greater than zero") }));

            _price = newPrice;
            return Result.Success();
        }

        public Result UpdateMinStockThreshold(int threshold)
        {
            if (threshold < 0)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("MinStockThreshold", "Minimum stock threshold cannot be negative") }));

            MinStockThreshold = threshold;

            // Check if current stock is already below the new threshold
            if (_stockQuantity <= MinStockThreshold)
            {
                // TODO: Handle low stock notification in the future
            }

            return Result.Success();
        }

        public Result Deactivate()
        {
            if (!IsActive)
                return Result.Failure(ErrorResponse.General("Variant is already inactive", "VARIANT_ALREADY_INACTIVE"));

            IsActive = false;
            return Result.Success();
        }

        public Result Activate()
        {
            if (IsActive)
                return Result.Failure(ErrorResponse.General("Variant is already active", "VARIANT_ALREADY_ACTIVE"));

            IsActive = true;
            return Result.Success();
        }

        public Result UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Name", "Name is required") }));

            if (name.Length > 200)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Name", "Name cannot exceed 200 characters") }));

            Name = name.Trim();
            return Result.Success();
        }

        private static List<ValidationError> ValidateInputs(string sku, string name, decimal? price, int minStockThreshold)
        {
            var errors = new List<ValidationError>();

            if (string.IsNullOrWhiteSpace(sku))
                errors.Add(new ValidationError("SKU", "SKU is required"));
            else if (sku.Length > 50)
                errors.Add(new ValidationError("SKU", "SKU cannot exceed 50 characters"));

            if (string.IsNullOrWhiteSpace(name))
                errors.Add(new ValidationError("Name", "Name is required"));
            else if (name.Length > 200)
                errors.Add(new ValidationError("Name", "Name cannot exceed 200 characters"));

            if (price.HasValue && price.Value <= 0)
                errors.Add(new ValidationError("Price", "Price must be greater than zero"));

            if (minStockThreshold < 0)
                errors.Add(new ValidationError("MinStockThreshold", "Minimum stock threshold cannot be negative"));

            return errors;
        }
    }
}