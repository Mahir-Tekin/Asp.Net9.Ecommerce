using Asp.Net9.Ecommerce.Domain.Common;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Catalog
{
    public class VariationType : AggregateRoot
    {
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public bool IsActive { get; private set; }

        private readonly List<VariantOption> _options = new();
        public IReadOnlyCollection<VariantOption> Options => _options.AsReadOnly();

        protected VariationType() { } // For EF Core

        public static Result<VariationType> Create(string name, string displayName)
        {
            var errors = ValidateInputs(name, displayName);
            if (errors.Any())
                return Result.Failure<VariationType>(ErrorResponse.ValidationError(errors));

            var type = new VariationType
            {
                Name = name.Trim().ToLowerInvariant(),
                DisplayName = displayName.Trim(),
                IsActive = true
            };

            return Result.Success(type);
        }

        public Result AddOption(string value, string displayValue)
        {
            if (_options.Any(o => o.Value == value))
            {
                return Result.Failure(ErrorResponse.ValidationError(
                    new List<ValidationError> { new("Value", "An option with this value already exists") }));
            }

            var option = VariantOption.Create(value, displayValue);
            _options.Add(option);
            return Result.Success();
        }

        public Result RemoveOption(string value)
        {
            var option = _options.FirstOrDefault(o => o.Value == value);
            if (option == null)
                return Result.NotFound("Option not found");

            _options.Remove(option);
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