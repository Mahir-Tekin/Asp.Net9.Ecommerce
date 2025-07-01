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

        // Navigation property for many-to-many with VariationType
        public ICollection<VariationType> VariationTypes { get; private set; } = new List<VariationType>();

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

        public Result AddVariationType(VariationType variationType)
        {
            if (variationType == null)
                return Result.Failure(ErrorResponse.ValidationError(new List<ValidationError> { new("VariationType", "Variation type is required") }));

            if (VariationTypes.Any(vt => vt.Id == variationType.Id))
                return Result.Failure(ErrorResponse.ValidationError(new List<ValidationError> { new("VariationType", "This variation type is already added") }));

            VariationTypes.Add(variationType);
            return Result.Success();
        }

        public Result UpdateVariationTypes(IEnumerable<VariationType> variationTypes)
        {
            if (variationTypes == null)
                return Result.Failure(ErrorResponse.ValidationError(new List<ValidationError> { new("VariationTypes", "Variation types are required") }));

            var newVariationTypes = variationTypes.ToList();
            
            // Check for duplicates in the input
            var duplicates = newVariationTypes.GroupBy(vt => vt.Id).Where(g => g.Count() > 1).Select(g => g.Key);
            if (duplicates.Any())
                return Result.Failure(ErrorResponse.ValidationError(new List<ValidationError> { new("VariationType", "Duplicate variation types provided") }));

            // Clear existing and add new ones
            VariationTypes.Clear();
            foreach (var variationType in newVariationTypes)
            {
                VariationTypes.Add(variationType);
            }
            
            return Result.Success();
        }

        private bool IsInHierarchy(Guid categoryId)
        {
            if (Id == categoryId) return true;
            return _subCategories.Any(sc => sc.IsInHierarchy(categoryId));
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