using System;

namespace Asp.Net9.Ecommerce.Application.Orders.DTOs
{
    public class AdminOrderListItemDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }
}
