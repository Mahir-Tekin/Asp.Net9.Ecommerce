using Asp.Net9.Ecommerce.Domain.Common;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Catalog
{
    public class Category : BaseEntity
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

        protected Category() { } // For EF Core

        public static Result<Category> Create(string name, string description, string slug, Guid? parentCategoryId = null)
        {
            var errors = ValidateInputs(name, description, slug);
            if (errors.Any())
                return Result.Failure<Category>(ErrorResponse.ValidationError(errors));

            return Result.Success(new Category
            {
                Name = name.Trim(),
                Description = description?.Trim(),
                Slug = slug.Trim().ToLowerInvariant(),
                IsActive = true,
                ParentCategoryId = parentCategoryId
            });
        }

        public Result Update(string name, string description, string slug)
        {
            var errors = ValidateInputs(name, description, slug);
            if (errors.Any())
                return Result.Failure(ErrorResponse.ValidationError(errors));

            Name = name.Trim();
            Description = description?.Trim();
            Slug = slug.Trim().ToLowerInvariant();

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