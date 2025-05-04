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
        
        // Physical Properties
        public decimal Weight { get; private set; }
        public string Dimensions { get; private set; }
        
        // Pricing - each variant can have its own price or inherit from product
        private decimal? _price;
        public decimal Price => _price ?? Product.BasePrice;
        private decimal? _discountedPrice;
        public decimal? DiscountedPrice => _discountedPrice ?? Product.DiscountedPrice;
        
        // Inventory
        private int _stockQuantity;
        public int StockQuantity => Product.TrackInventory ? _stockQuantity : 0;
        public int MinStockThreshold { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation property
        public Product Product { get; private set; }

        protected ProductVariant() { } // For EF Core

        public static Result<ProductVariant> Create(
            Guid productId,
            string sku,
            string name,
            decimal? price = null,
            int stockQuantity = 0,
            decimal weight = 0,
            string dimensions = null,
            int minStockThreshold = 10)
        {
            var errors = ValidateInputs(sku, name, price, weight, minStockThreshold);
            if (errors.Any())
                return Result.Failure<ProductVariant>(ErrorResponse.ValidationError(errors));

            var variant = new ProductVariant
            {
                ProductId = productId,
                SKU = sku.Trim().ToUpperInvariant(),
                Name = name.Trim(),
                _price = price,
                _stockQuantity = stockQuantity,
                Weight = weight,
                Dimensions = dimensions?.Trim(),
                MinStockThreshold = minStockThreshold,
                IsActive = true
            };

            return Result.Success(variant);
        }

        public Result UpdatePrice(decimal? newPrice)
        {
            if (newPrice.HasValue && newPrice.Value <= 0)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Price", "Price must be greater than zero") }));

            _price = newPrice;

            // If discounted price exists and is now higher than or equal to new price, remove it
            if (_price.HasValue && _discountedPrice.HasValue && _discountedPrice.Value >= _price.Value)
            {
                _discountedPrice = null;
            }

            return Result.Success();
        }

        public Result SetDiscount(decimal? discountedPrice)
        {
            if (discountedPrice.HasValue)
            {
                if (discountedPrice.Value <= 0)
                    return Result.Failure(ErrorResponse.ValidationError(
                        new List<ValidationError> { new("DiscountedPrice", "Discounted price must be greater than zero") }));

                var currentPrice = Price; // This will get either variant price or product price
                if (discountedPrice.Value >= currentPrice)
                    return Result.Failure(ErrorResponse.ValidationError(
                        new List<ValidationError> { new("DiscountedPrice", "Discounted price must be less than current price") }));
            }

            _discountedPrice = discountedPrice;
            return Result.Success();
        }

        public Result UpdateStock(int quantity)
        {
            if (!Product.TrackInventory)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("StockQuantity", "This product does not track inventory") }));

            if (quantity < 0)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("StockQuantity", "Stock quantity cannot be negative") }));

            var oldQuantity = _stockQuantity;
            _stockQuantity = quantity;

            // Check if stock is below threshold
            if (_stockQuantity <= MinStockThreshold)
            {
                AddDomainEvent(new VariantLowStockEvent(Id, ProductId, _stockQuantity, MinStockThreshold));
            }

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
                AddDomainEvent(new VariantLowStockEvent(Id, ProductId, _stockQuantity, MinStockThreshold));
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

        public Result UpdatePhysicalProperties(decimal weight, string dimensions)
        {
            if (weight < 0)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Weight", "Weight cannot be negative") }));

            Weight = weight;
            Dimensions = dimensions?.Trim();
            return Result.Success();
        }

        private static List<ValidationError> ValidateInputs(string sku, string name, decimal? price, decimal weight, int minStockThreshold)
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

            if (weight < 0)
                errors.Add(new ValidationError("Weight", "Weight cannot be negative"));

            if (minStockThreshold < 0)
                errors.Add(new ValidationError("MinStockThreshold", "Minimum stock threshold cannot be negative"));

            return errors;
        }
    }
} 