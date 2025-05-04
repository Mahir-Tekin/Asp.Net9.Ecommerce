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
        public string Slug { get; private set; }
        public string Brand { get; private set; }
        public string SKU { get; private set; }

        // Pricing
        public decimal BasePrice { get; private set; }
        public decimal? DiscountedPrice { get; private set; }

        // Inventory
        public bool TrackInventory { get; private set; }

        // Status
        public bool IsActive { get; private set; }
        public bool IsFeatured { get; private set; }
        public DateTime? PublishDate { get; private set; }
        public Guid CategoryId { get; private set; }

        // Navigation properties
        public Category Category { get; private set; }
        private readonly List<ProductVariant> _variants = new();
        public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();
        private readonly List<ProductImage> _images = new();
        public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

        protected Product() { } // For EF Core

        public static Result<Product> Create(
            string name,
            string description,
            string slug,
            string sku,
            decimal basePrice,
            Guid categoryId,
            bool trackInventory = true)
        {
            var errors = ValidateInputs(name, description, slug, basePrice, sku);
            if (errors.Any())
                return Result.Failure<Product>(ErrorResponse.ValidationError(errors));

            var product = new Product
            {
                Name = name.Trim(),
                Description = description?.Trim(),
                Slug = slug.Trim().ToLowerInvariant(),
                SKU = sku.Trim().ToUpperInvariant(),
                BasePrice = basePrice,
                CategoryId = categoryId,
                IsActive = true,
                TrackInventory = trackInventory
            };

            product.AddDomainEvent(new ProductCreatedEvent(product.Id, product.Name, product.Slug, product.BasePrice));

            return Result.Success(product);
        }

        public Result AddImage(string url, string altText, int? displayOrder = null, bool isMain = false)
        {
            var errors = new List<ValidationError>();

            if (string.IsNullOrWhiteSpace(url))
                errors.Add(new ValidationError("Url", "Image URL is required"));

            // Validate display order
            var actualDisplayOrder = displayOrder ?? _images.Count;
            if (_images.Any(i => i.DisplayOrder == actualDisplayOrder))
                errors.Add(new ValidationError("DisplayOrder", "An image with this display order already exists"));

            if (errors.Any())
                return Result.Failure(ErrorResponse.ValidationError(errors));
            
            // If this is the first image, make it main
            if (_images.Count == 0) isMain = true;

            // If this is marked as main, unmark other images
            if (isMain)
            {
                foreach (var image in _images.Where(i => i.IsMain))
                {
                    image.UnsetMain();
                }
            }

            var image = ProductImage.Create(Id, url, altText, actualDisplayOrder, isMain);
            _images.Add(image);

            return Result.Success();
        }

        public Result UpdateImageOrder(Guid imageId, int newDisplayOrder)
        {
            var image = _images.FirstOrDefault(i => i.Id == imageId);
            if (image == null)
                return Result.NotFound("Image not found");

            if (_images.Any(i => i.Id != imageId && i.DisplayOrder == newDisplayOrder))
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("DisplayOrder", "An image with this display order already exists") }));

            image.UpdateDisplayOrder(newDisplayOrder);
            return Result.Success();
        }

        public Result SetMainImage(Guid imageId)
        {
            var image = _images.FirstOrDefault(i => i.Id == imageId);
            if (image == null)
                return Result.NotFound("Image not found");

            foreach (var existingImage in _images)
            {
                if (existingImage.Id == imageId)
                    existingImage.SetMain();
                else
                    existingImage.UnsetMain();
            }

            return Result.Success();
        }

        public Result ReorderImages()
        {
            var orderedImages = _images.OrderBy(i => i.DisplayOrder).ToList();
            for (int i = 0; i < orderedImages.Count; i++)
            {
                orderedImages[i].UpdateDisplayOrder(i);
            }
            return Result.Success();
        }

        public Result SetFeatured(bool isFeatured)
        {
            IsFeatured = isFeatured;
            return Result.Success();
        }

        public Result SchedulePublishing(DateTime publishDate)
        {
            if (publishDate <= DateTime.UtcNow)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("PublishDate", "Publish date must be in the future") }));

            PublishDate = publishDate;
            return Result.Success();
        }

        public Result Update(string name, string description, string slug)
        {
            var errors = ValidateInputs(name, description, slug, BasePrice, SKU);
            if (errors.Any())
                return Result.Failure(ErrorResponse.ValidationError(errors));

            Name = name.Trim();
            Description = description?.Trim();
            Slug = slug.Trim().ToLowerInvariant();

            AddDomainEvent(new ProductUpdatedEvent(Id, Name, Slug));

            return Result.Success();
        }

        public Result UpdatePrice(decimal newBasePrice)
        {
            if (newBasePrice <= 0)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("BasePrice", "Price must be greater than zero") }));

            var oldPrice = BasePrice;
            BasePrice = newBasePrice;

            // If discounted price exists and is now higher than base price, remove it
            if (DiscountedPrice.HasValue && DiscountedPrice.Value >= newBasePrice)
            {
                DiscountedPrice = null;
            }

            AddDomainEvent(new ProductPriceChangedEvent(Id, oldPrice, newBasePrice));

            return Result.Success();
        }

        public Result SetDiscount(decimal? discountedPrice)
        {
            if (discountedPrice.HasValue)
            {
                if (discountedPrice.Value <= 0)
                    return Result.Failure(ErrorResponse.ValidationError(
                        new List<ValidationError> { new("DiscountedPrice", "Discounted price must be greater than zero") }));

                if (discountedPrice.Value >= BasePrice)
                    return Result.Failure(ErrorResponse.ValidationError(
                        new List<ValidationError> { new("DiscountedPrice", "Discounted price must be less than base price") }));
            }

            DiscountedPrice = discountedPrice;
            return Result.Success();
        }

        public Result Delete()
        {
            if (_variants.Any())
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("", "Cannot delete product with variants") }));

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

        private static List<ValidationError> ValidateInputs(string name, string description, string slug, decimal basePrice, string sku)
        {
            var errors = new List<ValidationError>();

            if (string.IsNullOrWhiteSpace(name))
                errors.Add(new ValidationError("Name", "Name is required"));
            else if (name.Length > 200)
                errors.Add(new ValidationError("Name", "Name cannot exceed 200 characters"));

            if (description?.Length > 2000)
                errors.Add(new ValidationError("Description", "Description cannot exceed 2000 characters"));

            if (string.IsNullOrWhiteSpace(slug))
                errors.Add(new ValidationError("Slug", "Slug is required"));
            else if (slug.Length > 200)
                errors.Add(new ValidationError("Slug", "Slug cannot exceed 200 characters"));
            else if (!IsValidSlug(slug))
                errors.Add(new ValidationError("Slug", "Slug contains invalid characters"));

            if (basePrice <= 0)
                errors.Add(new ValidationError("BasePrice", "Base price must be greater than zero"));

            if (string.IsNullOrWhiteSpace(sku))
                errors.Add(new ValidationError("SKU", "SKU is required"));
            else if (sku.Length > 50)
                errors.Add(new ValidationError("SKU", "SKU cannot exceed 50 characters"));

            return errors;
        }

        private static bool IsValidSlug(string slug)
        {
            return slug.All(c => char.IsLetterOrDigit(c) || c == '-');
        }
    }
} 