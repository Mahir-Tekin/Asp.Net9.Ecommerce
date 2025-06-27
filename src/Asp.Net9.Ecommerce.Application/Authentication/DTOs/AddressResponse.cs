namespace Asp.Net9.Ecommerce.Application.Authentication.DTOs
{
    /// <summary>
    /// Response DTO for user address
    /// </summary>
    public class AddressResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string City { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Neighborhood { get; set; } = null!;
        public string AddressLine { get; set; } = null!;
        public string AddressTitle { get; set; } = null!;
        public bool IsMain { get; set; }
        public bool IsDeleted { get; set; }
    }
}
