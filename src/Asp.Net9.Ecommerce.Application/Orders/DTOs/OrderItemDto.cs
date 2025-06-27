namespace Asp.Net9.Ecommerce.Application.Orders.DTOs
{
    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
        public Guid ProductVariantId { get; set; }
        public string ProductName { get; set; }
        public string ProductSlug { get; set; }
        public string? VariantName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? ImageUrl { get; set; }
    }
}
