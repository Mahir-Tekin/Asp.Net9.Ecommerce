using Asp.Net9.Ecommerce.Domain.Common;
using System.Collections.Generic;

namespace Asp.Net9.Ecommerce.Domain.Orders
{
    /// <summary>
    /// Value object for storing a snapshot of the shipping address in an order.
    /// </summary>
    public class OrderAddress : ValueObject
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string PhoneNumber { get; private set; }
        public string City { get; private set; }
        public string District { get; private set; }
        public string Neighborhood { get; private set; }
        public string AddressLine { get; private set; }
        public string AddressTitle { get; private set; }

        // EF Core requires a parameterless constructor
        private OrderAddress() { }

        public OrderAddress(
            string firstName,
            string lastName,
            string phoneNumber,
            string city,
            string district,
            string neighborhood,
            string addressLine,
            string addressTitle)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            City = city;
            District = district;
            Neighborhood = neighborhood;
            AddressLine = addressLine;
            AddressTitle = addressTitle;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
            yield return PhoneNumber;
            yield return City;
            yield return District;
            yield return Neighborhood;
            yield return AddressLine;
            yield return AddressTitle;
        }
    }
}
