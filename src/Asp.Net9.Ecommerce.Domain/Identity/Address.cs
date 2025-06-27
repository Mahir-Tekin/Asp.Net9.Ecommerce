using Asp.Net9.Ecommerce.Domain.Common;
using Asp.Net9.Ecommerce.Shared.Results;

namespace Asp.Net9.Ecommerce.Domain.Identity
{
    public class Address : BaseEntity
    {
        public Guid UserId { get; private set; } // FK to User/Customer
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string District { get; private set; } = string.Empty;
        public string Neighborhood { get; private set; } = string.Empty;
        public string AddressLine { get; private set; } = string.Empty;
        public string AddressTitle { get; private set; } = string.Empty;
        public bool IsMain { get; private set; } = false;

        protected Address() { } // For EF Core

        public static Result<Address> Create(
            Guid userId,
            string firstName,
            string lastName,
            string phoneNumber,
            string city,
            string district,
            string neighborhood,
            string addressLine,
            string addressTitle)
        {
            var errors = new List<ValidationError>();
            if (string.IsNullOrWhiteSpace(firstName))
                errors.Add(new ValidationError("FirstName", "First name is required"));
            if (string.IsNullOrWhiteSpace(lastName))
                errors.Add(new ValidationError("LastName", "Last name is required"));
            if (string.IsNullOrWhiteSpace(phoneNumber))
                errors.Add(new ValidationError("PhoneNumber", "Phone number is required"));
            if (string.IsNullOrWhiteSpace(city))
                errors.Add(new ValidationError("City", "City is required"));
            if (string.IsNullOrWhiteSpace(district))
                errors.Add(new ValidationError("District", "District is required"));
            if (string.IsNullOrWhiteSpace(neighborhood))
                errors.Add(new ValidationError("Neighborhood", "Neighborhood is required"));
            if (string.IsNullOrWhiteSpace(addressLine))
                errors.Add(new ValidationError("AddressLine", "Address is required"));
            if (string.IsNullOrWhiteSpace(addressTitle))
                errors.Add(new ValidationError("AddressTitle", "Address title is required"));
            if (errors.Any())
                return Result.Failure<Address>(ErrorResponse.ValidationError(errors));

            var address = new Address
            {
                UserId = userId,
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                PhoneNumber = phoneNumber.Trim(),
                City = city.Trim(),
                District = district.Trim(),
                Neighborhood = neighborhood.Trim(),
                AddressLine = addressLine.Trim(),
                AddressTitle = addressTitle.Trim()
            };
            address.SetCreated();
            return Result.Success(address);
        }

        public Result Update(
            string firstName,
            string lastName,
            string phoneNumber,
            string city,
            string district,
            string neighborhood,
            string addressLine,
            string addressTitle)
        {
            var errors = new List<ValidationError>();
            if (string.IsNullOrWhiteSpace(firstName))
                errors.Add(new ValidationError("FirstName", "First name is required"));
            if (string.IsNullOrWhiteSpace(lastName))
                errors.Add(new ValidationError("LastName", "Last name is required"));
            if (string.IsNullOrWhiteSpace(phoneNumber))
                errors.Add(new ValidationError("PhoneNumber", "Phone number is required"));
            if (string.IsNullOrWhiteSpace(city))
                errors.Add(new ValidationError("City", "City is required"));
            if (string.IsNullOrWhiteSpace(district))
                errors.Add(new ValidationError("District", "District is required"));
            if (string.IsNullOrWhiteSpace(neighborhood))
                errors.Add(new ValidationError("Neighborhood", "Neighborhood is required"));
            if (string.IsNullOrWhiteSpace(addressLine))
                errors.Add(new ValidationError("AddressLine", "Address is required"));
            if (string.IsNullOrWhiteSpace(addressTitle))
                errors.Add(new ValidationError("AddressTitle", "Address title is required"));
            if (errors.Any())
                return Result.Failure(ErrorResponse.ValidationError(errors));

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            PhoneNumber = phoneNumber.Trim();
            City = city.Trim();
            District = district.Trim();
            Neighborhood = neighborhood.Trim();
            AddressLine = addressLine.Trim();
            AddressTitle = addressTitle.Trim();
            SetUpdated();
            return Result.Success();
        }

        public Result SoftDelete()
        {
            if (IsDeleted)
                return Result.Failure(ErrorResponse.General("Address is already deleted", "ADDRESS_ALREADY_DELETED"));
            SetDeleted();
            return Result.Success();
        }

        public void SetAsMain() => IsMain = true;
        public void UnsetAsMain() => IsMain = false;
    }
}
