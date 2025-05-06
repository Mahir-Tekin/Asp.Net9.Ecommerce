using Asp.Net9.Ecommerce.Domain.Common;
using Asp.Net9.Ecommerce.Domain.Catalog.Events;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Catalog
{
    public class ProductVariant : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public string SKU { get; private set; }
        public string Name { get; private set; }
        
        // Pricing - each variant can have its own price or inherit from product
        private decimal? _price;
        public decimal Price => _price ?? Product.BasePrice;
        
        // Inventory
        private int _stockQuantity;
        public int StockQuantity => _stockQuantity;
        public int MinStockThreshold { get; private set; }
        public bool IsActive { get; private set; }
        public bool TrackInventory { get; private set; }

        // Variations - stores type name and option value
        private readonly Dictionary<string, string> _variations = new();

        // Navigation property
        public Product Product { get; private set; }

        protected ProductVariant() { } // For EF Core

        public static Result<ProductVariant> Create(
            Guid productId,
            string sku,
            string name,
            decimal? price = null,
            int stockQuantity = 0,
            int minStockThreshold = 10,
            bool trackInventory = true)
        {
            var errors = ValidateInputs(sku, name, price, minStockThreshold);
            if (errors.Any())
                return Result.Failure<ProductVariant>(ErrorResponse.ValidationError(errors));

            var variant = new ProductVariant
            {
                ProductId = productId,
                SKU = sku.Trim().ToUpperInvariant(),
                Name = name.Trim(),
                _price = price,
                _stockQuantity = stockQuantity,
                MinStockThreshold = minStockThreshold,
                IsActive = true,
                TrackInventory = trackInventory
            };

            return Result.Success(variant);
        }

        public Result AddVariation(string typeName, string optionValue)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("TypeName", "Variant type name is required") }));

            if (string.IsNullOrWhiteSpace(optionValue))
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("OptionValue", "Option value is required") }));

            _variations[typeName.Trim().ToLowerInvariant()] = optionValue.Trim().ToLowerInvariant();
            return Result.Success();
        }

        public Result RemoveVariation(string typeName)
        {
            if (!_variations.ContainsKey(typeName.Trim().ToLowerInvariant()))
                return Result.NotFound($"Variation type {typeName} not found");

            _variations.Remove(typeName.Trim().ToLowerInvariant());
            return Result.Success();
        }

        public bool HasVariation(string typeName)
        {
            return _variations.ContainsKey(typeName.Trim().ToLowerInvariant());
        }

        public string? GetVariationOption(string typeName)
        {
            return _variations.TryGetValue(typeName.Trim().ToLowerInvariant(), out var value) ? value : null;
        }

        public IReadOnlyDictionary<string, string> GetVariations()
        {
            return _variations.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value,
                StringComparer.OrdinalIgnoreCase
            );
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