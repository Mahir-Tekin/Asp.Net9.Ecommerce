using Asp.Net9.Ecommerce.Shared.Results;
using MediatR;

namespace Asp.Net9.Ecommerce.Application.Authentication.Commands.Address
{
    /// <summary>
    /// Command to create a new address for the current user.
    /// </summary>
    public class CreateAddressCommand : IRequest<Result<Guid>>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string City { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Neighborhood { get; set; } = null!;
        public string AddressLine { get; set; } = null!;
        public string AddressTitle { get; set; } = null!;
        // Internal use only
        public Guid UserId { get; set; }
    }
}
