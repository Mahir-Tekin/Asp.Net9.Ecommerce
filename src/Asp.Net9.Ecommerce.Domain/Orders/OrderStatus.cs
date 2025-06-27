namespace Asp.Net9.Ecommerce.Domain.Orders
{
    public enum OrderStatus
    {
        Pending = 0,
        Paid = 1,
        Shipped = 2,
        Delivered = 3,
        Cancelled = 4
    }
}
