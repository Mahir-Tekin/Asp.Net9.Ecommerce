using Asp.Net9.Ecommerce.Domain.Common;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Catalog
{
    public class VariationType : AggregateRoot
    {
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public bool IsActive { get; private set; }

        public ICollection<VariantOption> Options { get; private set; } = new List<VariantOption>();

        // Navigation property for many-to-many with Category
        public ICollection<Category> Categories { get; private set; } = new List<Category>();

        protected VariationType() { } // For EF Core

        public static Result<VariationType> Create(string name, string displayName)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<VariationType>(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Name", "Name is required") }));

            if (string.IsNullOrWhiteSpace(displayName))
                return Result.Failure<VariationType>(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("DisplayName", "Display name is required") }));

            var type = new VariationType
            {
                Name = name.Trim().ToLowerInvariant(),
                DisplayName = displayName.Trim(),
                IsActive = true
            };

            return Result.Success(type);
        }

        public Result UpdateName(string name, string displayName)
        {
            var errors = ValidateInputs(name, displayName);
            if (errors.Any())
                return Result.Failure(ErrorResponse.ValidationError(errors));

            Name = name.Trim().ToLowerInvariant();
            DisplayName = displayName.Trim();
            
            return Result.Success();
        }

        public Result AddOption(string value, string displayValue, int sortOrder = 0)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Value", "Option value is required") }));

            if (string.IsNullOrWhiteSpace(displayValue))
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("DisplayValue", "Option display value is required") }));

            if (Options.Any(o => o.Value == value.Trim().ToLowerInvariant()))
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Value", "This option value already exists") }));

            // Ensure VariationType has an Id before adding options
            if (this.Id == Guid.Empty)
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("VariationTypeId", "VariationType must have a valid Id before adding options.") }));

            var optionResult = VariantOption.Create(value, displayValue, sortOrder, this.Id);
            if (optionResult.IsFailure)
                return Result.Failure(optionResult.Error);

            Options.Add(optionResult.Value);
            return Result.Success();
        }

        public Result RemoveOption(string value)
        {
            var option = Options.FirstOrDefault(o => o.Value == value.Trim().ToLowerInvariant());
            if (option == null)
                return Result.NotFound("Option not found");

            Options.Remove(option);
            return Result.Success();
        }

        public Result RemoveOptionById(Guid optionId)
        {
            var option = Options.FirstOrDefault(o => o.Id == optionId);
            if (option == null)
                return Result.NotFound("Option not found");

            Options.Remove(option);
            return Result.Success();
        }

        public Result UpdateDisplayName(string displayName)
        {
            var errors = ValidateInputs(Name, displayName);
            if (errors.Any())
                return Result.Failure(ErrorResponse.ValidationError(errors));

            DisplayName = displayName.Trim();
            return Result.Success();
        }

        public Result Deactivate()
        {
            if (!IsActive)
                return Result.Failure(ErrorResponse.General("Variant type is already inactive", "VARIANT_TYPE_ALREADY_INACTIVE"));

            IsActive = false;
            return Result.Success();
        }

        public Result Activate()
        {
            if (IsActive)
                return Result.Failure(ErrorResponse.General("Variant type is already active", "VARIANT_TYPE_ALREADY_ACTIVE"));

            IsActive = true;
            return Result.Success();
        }

        public Result UpdateOption(Guid optionId, string value, string displayValue, int sortOrder)
        {
            var option = Options.FirstOrDefault(o => o.Id == optionId);
            if (option == null)
                return Result.NotFound("Option not found");

            // Check if the new value conflicts with other options (excluding this one)
            if (Options.Any(o => o.Id != optionId && o.Value == value.Trim().ToLowerInvariant()))
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Value", "This option value already exists") }));

            var updateResult = option.Update(value, displayValue, sortOrder);
            return updateResult;
        }

        private static List<ValidationError> ValidateInputs(string name, string displayName)
        {
            var errors = new List<ValidationError>();

            if (string.IsNullOrWhiteSpace(name))
                errors.Add(new ValidationError("Name", "Name is required"));
            else if (name.Length > 50)
                errors.Add(new ValidationError("Name", "Name cannot exceed 50 characters"));

            if (string.IsNullOrWhiteSpace(displayName))
                errors.Add(new ValidationError("DisplayName", "Display name is required"));
            else if (displayName.Length > 100)
                errors.Add(new ValidationError("DisplayName", "Display name cannot exceed 100 characters"));

            return errors;
        }
    }
}