using System;
using System.Collections.Generic;

namespace Asp.Net9.Ecommerce.Application.Orders.DTOs
{
    public class OrderSummaryDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> Items { get; set; }
        public OrderAddressDto? ShippingAddress { get; set; }
    }
}
