namespace Asp.Net9.Ecommerce.Application.Orders.DTOs
{
    public class CreateOrderDto
    {
        public OrderAddressDto ShippingAddress { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }

    public class OrderAddressDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Neighborhood { get; set; }
        public string AddressLine { get; set; }
        public string AddressTitle { get; set; }
    }


}
