namespace Asp.Net9.Ecommerce.Application.Orders.DTOs
{
    public class UpdateOrderStatusDto
    {
        public string NewStatus { get; set; }
        public string? Comment { get; set; }
    }
}
