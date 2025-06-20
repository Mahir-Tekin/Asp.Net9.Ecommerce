using Asp.Net9.Ecommerce.Domain.Common;
using Asp.Net9.Ecommerce.Domain.Catalog.Events;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Catalog
{
    public class Category : AggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Slug { get; private set; }
        public bool IsActive { get; private set; }
        public Guid? ParentCategoryId { get; private set; }

        // Navigation property
        public Category ParentCategory { get; private set; }
        private readonly List<Category> _subCategories = new();
        public IReadOnlyCollection<Category> SubCategories => _subCategories.AsReadOnly();

        // Variation types
        private readonly List<CategoryVariationType> _variationTypes = new();
        public IReadOnlyCollection<CategoryVariationType> VariationTypes => _variationTypes.AsReadOnly();

        protected Category() { } // For EF Core

        public static Result<Category> Create(string name, string description, string slug, Guid? parentCategoryId = null)
        {
            var errors = ValidateInputs(name, description, slug);
            if (errors.Any())
                return Result.Failure<Category>(ErrorResponse.ValidationError(errors));

            var category = new Category
            {
                Name = name.Trim(),
                Description = description?.Trim(),
                Slug = slug.Trim().ToLowerInvariant(),
                IsActive = true,
                ParentCategoryId = parentCategoryId
            };

            // Add domain event
            category.AddDomainEvent(new CategoryCreatedEvent(category.Id, category.Name, category.Slug));

            return Result.Success(category);
        }

        public Result Update(string name, string description, string slug)
        {
            var errors = ValidateInputs(name, description, slug);
            if (errors.Any())
                return Result.Failure(ErrorResponse.ValidationError(errors));

            Name = name.Trim();
            Description = description?.Trim();
            Slug = slug.Trim().ToLowerInvariant();

            // Add domain event
            AddDomainEvent(new CategoryUpdatedEvent(Id, Name, Slug));

            return Result.Success();
        }

        public Result Deactivate()
        {
            if (!IsActive)
                return Result.Failure(ErrorResponse.General("Category is already inactive", "CATEGORY_ALREADY_INACTIVE"));

            IsActive = false;
            return Result.Success();
        }

        public Result Activate()
        {
            if (IsActive)
                return Result.Failure(ErrorResponse.General("Category is already active", "CATEGORY_ALREADY_ACTIVE"));

            IsActive = true;
            return Result.Success();
        }

        public Result Delete()
        {
            if (_subCategories.Any())
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("", "Cannot delete category with subcategories") }));

            SetDeleted();
            AddDomainEvent(new CategoryDeletedEvent(Id));
            return Result.Success();
        }

        public void AddSubCategory(Category subCategory)
        {
            if (subCategory == null)
                throw new ArgumentNullException(nameof(subCategory));

            if (_subCategories.Any(sc => sc.Id == subCategory.Id))
                return; // Skip if subcategory already exists

            // Check for circular reference
            if (IsInHierarchy(subCategory.Id))
                throw new InvalidOperationException("Circular reference detected in category hierarchy");
                
            _subCategories.Add(subCategory);
        }

        private bool IsInHierarchy(Guid categoryId)
        {
            if (Id == categoryId) return true;
            return _subCategories.Any(sc => sc.IsInHierarchy(categoryId));
        }

        public Result AddVariationType(Guid variationTypeId, bool isRequired)
        {
            if (_variationTypes.Any(vt => vt.VariationTypeId == variationTypeId))
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("VariationType", "This variation type is already added") }));

            var variationTypeResult = CategoryVariationType.Create(variationTypeId, isRequired);
            if (variationTypeResult.IsFailure)
                return Result.Failure(variationTypeResult.Error);

            _variationTypes.Add(variationTypeResult.Value);
            return Result.Success();
        }

        public Result RemoveVariationType(Guid variationTypeId)
        {
            var variationType = _variationTypes.FirstOrDefault(vt => vt.VariationTypeId == variationTypeId);
            if (variationType == null)
                return Result.NotFound("Variation type not found");

            _variationTypes.Remove(variationType);
            return Result.Success();
        }

        public IReadOnlyCollection<CategoryVariationType> GetEffectiveVariationTypes()
        {
            var variations = new List<CategoryVariationType>();
            
            // Add parent variations if exists
            if (ParentCategory != null)
            {
                variations.AddRange(ParentCategory.GetEffectiveVariationTypes());
            }
            
            // Add this category's variations
            variations.AddRange(_variationTypes);
            
            return variations;
        }

        public Result UpdateVariationTypes(List<CategoryVariationType> variationTypes)
        {
            // Clear existing variation types
            _variationTypes.Clear();

            // Add new variation types
            foreach (var variationType in variationTypes)
            {
                if (_variationTypes.Any(vt => vt.VariationTypeId == variationType.VariationTypeId))
                    return Result.Failure(ErrorResponse.ValidationError(
                        new List<ValidationError> { new("VariationType", "Duplicate variation type found") }));

                _variationTypes.Add(variationType);
            }

            // Add domain event
            AddDomainEvent(new CategoryVariationTypesUpdatedEvent(Id, variationTypes.Select(vt => vt.VariationTypeId).ToList()));

            return Result.Success();
        }

        private static List<ValidationError> ValidateInputs(string name, string description, string slug)
        {
            var errors = new List<ValidationError>();

            if (string.IsNullOrWhiteSpace(name))
                errors.Add(new ValidationError("Name", "Name is required"));
            else if (name.Length > 100)
                errors.Add(new ValidationError("Name", "Name cannot exceed 100 characters"));

            if (description?.Length > 500)
                errors.Add(new ValidationError("Description", "Description cannot exceed 500 characters"));

            if (string.IsNullOrWhiteSpace(slug))
                errors.Add(new ValidationError("Slug", "Slug is required"));
            else if (slug.Length > 100)
                errors.Add(new ValidationError("Slug", "Slug cannot exceed 100 characters"));
            else if (!IsValidSlug(slug))
                errors.Add(new ValidationError("Slug", "Slug contains invalid characters"));

            return errors;
        }

        private static bool IsValidSlug(string slug)
        {
            return slug.All(c => char.IsLetterOrDigit(c) || c == '-');
        }
    }
} 