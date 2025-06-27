using Asp.Net9.Ecommerce.Domain.Common;
using System.Collections.Generic;

namespace Asp.Net9.Ecommerce.Domain.Orders
{
    /// <summary>
    /// Aggregate root for an order.
    /// </summary>
    public class Order : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public OrderAddress ShippingAddress { get; private set; }
        public List<OrderItem> Items { get; private set; } = new();
        public OrderStatus Status { get; private set; }
        public decimal TotalAmount { get; private set; }

        // EF Core requires a parameterless constructor
        private Order() { }

        public Order(Guid userId, OrderAddress shippingAddress, List<OrderItem> items)
        {
            UserId = userId;
            ShippingAddress = shippingAddress;
            Items = items;
            Status = OrderStatus.Pending;
            TotalAmount = items.Sum(i => i.TotalPrice);
            SetCreated();
        }

        public void SetStatus(OrderStatus status)
        {
            Status = status;
            SetUpdated();
        }
    }
}
