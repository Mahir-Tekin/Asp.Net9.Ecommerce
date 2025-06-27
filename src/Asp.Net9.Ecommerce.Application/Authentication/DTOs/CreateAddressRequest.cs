namespace Asp.Net9.Ecommerce.Application.Authentication.DTOs
{
    /// <summary>
    /// Request model for creating a new address for a user.
    /// </summary>
    public class CreateAddressRequest
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string City { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Neighborhood { get; set; } = null!;
        public string AddressLine { get; set; } = null!;
        public string AddressTitle { get; set; } = null!;
    }
}
