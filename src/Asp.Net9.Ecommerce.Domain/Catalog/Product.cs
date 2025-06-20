using Asp.Net9.Ecommerce.Domain.Common;
using Asp.Net9.Ecommerce.Domain.Catalog.Events;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Catalog
{
    public class Product : AggregateRoot
    {
        // Basic Information
        public string? Name { get; private set; }
        public string? Description { get; private set; }
        public Guid CategoryId { get; private set; }

        // Pricing
        public decimal BasePrice { get; private set; }

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
        public Category? Category { get; private set; }
        private readonly List<ProductVariant> _variants = new();
        public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

        protected Product() { } // For EF Core



        /// <summary>
        /// Data structure for holding variant information before creating ProductVariant entities.
        /// Used as input to the Product.Create factory method.
        /// </summary>
        public record VariantData
        {
            /// <summary>SKU for the variant (must be unique per product)</summary>
            public string SKU { get; init; } = string.Empty;
            /// <summary>Name of the variant</summary>
            public string Name { get; init; } = string.Empty;
            /// <summary>Optional price override for the variant</summary>
            public decimal? Price { get; init; }
            /// <summary>
            /// Selected options for this variant. Key: VariationTypeId, Value: VariantOptionId.
            /// </summary>
            public IDictionary<Guid, Guid> SelectedOptions { get; init; } = new Dictionary<Guid, Guid>();
        }

        /// <summary>
        /// Data structure for holding image information before creating ProductImage entities.
        /// Used as input to the Product.Create factory method.
        /// </summary>
        public record ImageData
        {
            public string Url { get; init; } = string.Empty;
            public string? AltText { get; init; }
            public bool IsMain { get; init; }
        }


        /// <summary>
        /// Factory method to create a new Product aggregate with variants, images, and variant types.
        /// Handles all domain validation and construction of child entities.
        /// </summary>
        /// <param name="name">Product name</param>
        /// <param name="description">Product description</param>
        /// <param name="basePrice">Base price for the product</param>
        /// <param name="categoryId">Category ID (foreign key)</param>
        /// <param name="variantTypes">List of VariationType entities (must be loaded by handler)</param>
        /// <param name="variantData">List of VariantData (raw variant info from handler/DTO)</param>
        /// <param name="images">Optional images for the product</param>
        /// <returns>Result containing the created Product or validation errors</returns>
        public static Result<Product> Create(
            string name,
            string description,
            decimal basePrice,
            Guid categoryId,
            IEnumerable<VariationType>? variantTypes,
            IEnumerable<VariantData> variantData,
            IEnumerable<ImageData>? images = null)
        {
            // 1. Validate basic product info
            var errors = ValidateInputs(name, description, basePrice);
            if (errors.Any())
                return Result.Failure<Product>(ErrorResponse.ValidationError(errors));

            // 2. Validate at least one variant is provided
            var variantDataList = variantData.ToList();
            if (!variantDataList.Any())
                return Result.Failure<Product>(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Variants", "At least one variant is required") }));

            // 3. Create the Product entity (aggregate root)
            var product = new Product
            {
                Name = name.Trim(),
                Description = description!.Trim(),
                BasePrice = basePrice,
                CategoryId = categoryId,
                IsActive = true
            };


            // 4. Add images (if provided)
            if (images != null && images.Any())
            {
                int mainCount = images.Count(i => i.IsMain);
                if (mainCount > 1)
                {
                    return Result.Failure<Product>(ErrorResponse.ValidationError(
                        new List<ValidationError> { new("Images", "Only one image can be marked as main.") }));
                }
                foreach (var img in images)
                {
                    var imgResult = ProductImage.Create(img.Url, img.AltText, img.IsMain);
                    if (imgResult.IsFailure)
                        return Result.Failure<Product>(imgResult.Error);
                    product._images.Add(imgResult.Value);
                }
            }


            // 5. Add variant types (must be loaded entities)
            // Handler should load VariationType entities from DB and pass them in.
            // This ensures we can validate their state and use their options for variant validation.
            if (variantTypes != null)
            {
                var variantTypesList = variantTypes.ToList();
                foreach (var type in variantTypesList)
                {
                    // Validate that the variant type is active (business rule)
                    if (!type.IsActive)
                        return Result.Failure<Product>(ErrorResponse.ValidationError(
                            new List<ValidationError> { new("VariantType", $"Variant type {type.Name} is not active") }));

                    // Add the variant type to the product's collection
                    product._variantTypes.Add(type);
                }
            }


            // 6. Check for duplicate SKUs in variant data
            var duplicateSKUs = variantDataList
                .GroupBy(v => v.SKU)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateSKUs.Any())
                return Result.Failure<Product>(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Variants", 
                        $"Duplicate SKUs found: {string.Join(", ", duplicateSKUs)}") }));

            // 7. Create and add ProductVariant entities for each variant
            foreach (var data in variantDataList)
            {
                // Validate all required variant types are provided in this variant
                if (product._variantTypes.Any())
                {
                    var missingTypes = product._variantTypes
                        .Select(vt => vt.Id)
                        .Except(data.SelectedOptions.Keys)
                        .ToList();

                    if (missingTypes.Any())
                        return Result.Failure<Product>(ErrorResponse.ValidationError(
                            new List<ValidationError> { new("Variant",
                                $"Variant with SKU '{data.SKU}' is missing options for variant types: {string.Join(", ", missingTypes)}") }));
                }

                // Create the ProductVariant entity with selected options and variant types
                var variantResult = ProductVariant.Create(
                    productId: product.Id,
                    sku: data.SKU,
                    name: data.Name,
                    price: data.Price,
                    variantTypes: product._variantTypes,
                    selectedOptions: data.SelectedOptions
                );

                if (variantResult.IsFailure)
                    return Result.Failure<Product>(variantResult.Error);

                product._variants.Add(variantResult.Value);
            }

            // 9. Add domain event for product creation
            product.AddDomainEvent(new ProductCreatedEvent(product.Id, product.Name, variantDataList.First().SKU, product.BasePrice));

            // 10. Return the created product
            return Result.Success(product);
        }

        public Result AddVariantType(VariationType type)
        {
            if (!type.IsActive)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("VariantType", "Variant type is not active") }));

            if (_variantTypes.Any(t => t.Id == type.Id))
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("VariantType", "This variant type is already added") }));

            _variantTypes.Add(type);
            return Result.Success();
        }

        public Result RemoveVariantType(Guid variantTypeId)
        {
            // Can only remove variant types if there are no variants
            if (_variants.Any())
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("VariantType", 
                        "Cannot remove variant type when product has variants. All variants must maintain all variant types.") }));

            var type = _variantTypes.FirstOrDefault(t => t.Id == variantTypeId);
            if (type == null)
                return Result.NotFound("Variant type not found");

            _variantTypes.Remove(type);
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

            // Enforce only one main image
            if (isMain && _images.Any(i => i.IsMain))
            {
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Images", "Only one image can be marked as main.") }));
            }

            var imageResult = ProductImage.Create(url, altText, isMain, this.Id);
            if (imageResult.IsFailure)
                return Result.Failure(imageResult.Error);

            var image = imageResult.Value;
            // Set Product navigation and ProductId
            typeof(ProductImage).GetProperty("Product")?.SetValue(image, this);
            typeof(ProductImage).GetProperty("ProductId")?.SetValue(image, this.Id);

            _images.Add(image);

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