using Asp.Net9.Ecommerce.Domain.Common;
using Asp.Net9.Ecommerce.Domain.Catalog.Events;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Catalog
{
    public class Product : AggregateRoot
    {
        // Basic Information
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Guid CategoryId { get; private set; }

        // Pricing
        public decimal BasePrice { get; private set; }
        public decimal? OldPrice { get; private set; }

        // Images
        private readonly List<ProductImage> _images = new();
        public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();
        public string? MainImage => _images.FirstOrDefault(x => x.IsMain)?.Url ?? _images.FirstOrDefault()?.Url;

        // Variant Types
        private readonly List<VariationType> _variantTypes = new();
        public IReadOnlyCollection<VariationType> VariantTypes => _variantTypes.AsReadOnly();

        // Status
        public bool IsActive { get; private set; }

        // Navigation properties
        public Category Category { get; private set; }
        private readonly List<ProductVariant> _variants = new();
        public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

        protected Product() { } // For EF Core

        public static Result<Product> Create(
            string name,
            string description,
            decimal basePrice,
            Guid categoryId,
            ProductVariant defaultVariant,
            IEnumerable<VariationType>? variantTypes = null)
        {
            var errors = ValidateInputs(name, description, basePrice);
            if (errors.Any())
                return Result.Failure<Product>(ErrorResponse.ValidationError(errors));

            var product = new Product
            {
                Name = name.Trim(),
                Description = description?.Trim(),
                BasePrice = basePrice,
                CategoryId = categoryId,
                IsActive = true
            };

            // Add variant types if provided
            if (variantTypes != null)
            {
                foreach (var type in variantTypes)
                {
                    if (!type.IsActive)
                        return Result.Failure<Product>(ErrorResponse.ValidationError(
                            new List<ValidationError> { new("VariantType", $"Variant type {type.Name} is not active") }));

                    product._variantTypes.Add(type);
                }
            }

            // Add the default variant
            product._variants.Add(defaultVariant);

            product.AddDomainEvent(new ProductCreatedEvent(product.Id, product.Name, defaultVariant.SKU, product.BasePrice));

            return Result.Success(product);
        }

        public Result AddVariantType(VariationType type)
        {
            if (!type.IsActive)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("VariantType", "Variant type is not active") }));

            if (_variantTypes.Any(t => t.Name == type.Name))
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("VariantType", "This variant type is already added") }));

            _variantTypes.Add(type);
            return Result.Success();
        }

        public Result RemoveVariantType(string typeName)
        {
            var type = _variantTypes.FirstOrDefault(t => t.Name == typeName);
            if (type == null)
                return Result.NotFound("Variant type not found");

            // Check if any variants are using this type
            if (_variants.Any(v => v.HasVariation(typeName)))
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("VariantType", "Cannot remove variant type that is in use") }));

            _variantTypes.Remove(type);
            return Result.Success();
        }

        public Result AddVariant(ProductVariant variant)
        {
            // Validate that all variant's options belong to product's variant types
            foreach (var variation in variant.Variations)
            {
                var type = _variantTypes.FirstOrDefault(t => t.Name == variation.Key);
                if (type == null)
                    return Result.Failure(ErrorResponse.ValidationError(
                        new List<ValidationError> { new("Variation", $"Variant type {variation.Key} is not defined for this product") }));

                if (!type.Options.Any(o => o.Value == variation.Value))
                    return Result.Failure(ErrorResponse.ValidationError(
                        new List<ValidationError> { new("Variation", $"Option {variation.Value} is not valid for variant type {variation.Key}") }));
            }

            // Check for duplicate SKU
            if (_variants.Any(v => v.SKU == variant.SKU))
            {
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("SKU", "A variant with this SKU already exists") }));
            }
            
            _variants.Add(variant);
            return Result.Success();
        }

        public Result Update(string name, string description)
        {
            var errors = ValidateInputs(name, description, BasePrice);
            if (errors.Any())
                return Result.Failure(ErrorResponse.ValidationError(errors));

            Name = name.Trim();
            Description = description?.Trim();

            AddDomainEvent(new ProductUpdatedEvent(Id, Name, _variants.First().SKU));

            return Result.Success();
        }

        public Result UpdatePrice(decimal newBasePrice)
        {
            if (newBasePrice <= 0)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("BasePrice", "Price must be greater than zero") }));

            var oldPrice = BasePrice;
            OldPrice = oldPrice;
            BasePrice = newBasePrice;

            AddDomainEvent(new ProductPriceChangedEvent(Id, oldPrice, newBasePrice));

            return Result.Success();
        }

        public Result Delete()
        {
            SetDeleted();
            AddDomainEvent(new ProductDeletedEvent(Id));
            return Result.Success();
        }

        public Result Deactivate()
        {
            if (!IsActive)
                return Result.Failure(ErrorResponse.General("Product is already inactive", "PRODUCT_ALREADY_INACTIVE"));

            IsActive = false;
            return Result.Success();
        }

        public Result Activate()
        {
            if (IsActive)
                return Result.Failure(ErrorResponse.General("Product is already active", "PRODUCT_ALREADY_ACTIVE"));

            IsActive = true;
            return Result.Success();
        }

        public Result AddImage(string url, string? altText = null, bool isMain = false)
        {
            if (string.IsNullOrWhiteSpace(url))
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Url", "Image URL is required") }));

            // If this is the first image, make it main
            if (!_images.Any()) 
                isMain = true;

            // If this is marked as main, unmark other images
            if (isMain)
            {
                foreach (var existingImage in _images.Where(i => i.IsMain))
                {
                    existingImage.UnsetMain();
                }
            }

            var imageResult = ProductImage.Create(url, altText, isMain);
            if (imageResult.IsFailure)
                return Result.Failure(imageResult.Error);

            _images.Add(imageResult.Value);

            return Result.Success();
        }

        public Result RemoveImage(string url)
        {
            var image = _images.FirstOrDefault(i => i.Url == url);
            if (image == null)
                return Result.NotFound("Image not found");

            _images.Remove(image);

            // If we removed the main image and there are other images, make the first one main
            if (image.IsMain && _images.Any())
            {
                _images.First().SetMain();
            }

            return Result.Success();
        }

        public Result SetMainImage(string url)
        {
            var image = _images.FirstOrDefault(i => i.Url == url);
            if (image == null)
                return Result.NotFound("Image not found");

            foreach (var existingImage in _images)
            {
                if (existingImage.Url == url)
                    existingImage.SetMain();
                else
                    existingImage.UnsetMain();
            }

            return Result.Success();
        }

        private static List<ValidationError> ValidateInputs(string name, string description, decimal basePrice)
        {
            var errors = new List<ValidationError>();

            if (string.IsNullOrWhiteSpace(name))
                errors.Add(new ValidationError("Name", "Name is required"));
            else if (name.Length > 200)
                errors.Add(new ValidationError("Name", "Name cannot exceed 200 characters"));

            if (description?.Length > 2000)
                errors.Add(new ValidationError("Description", "Description cannot exceed 2000 characters"));

            if (basePrice <= 0)
                errors.Add(new ValidationError("BasePrice", "Base price must be greater than zero"));

            return errors;
        }
    }
} 