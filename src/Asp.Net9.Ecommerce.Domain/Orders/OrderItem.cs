using Asp.Net9.Ecommerce.Domain.Common;

namespace Asp.Net9.Ecommerce.Domain.Orders
{
    /// <summary>
    /// Entity representing a single item in an order (product snapshot).
    /// </summary>
    public class OrderItem : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public Guid ProductVariantId { get; private set; }
        public string ProductName { get; private set; } = string.Empty;
        public string ProductSlug { get; private set; } = string.Empty;
        public string? VariantName { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        public string? ImageUrl { get; private set; }

        // EF Core requires a parameterless constructor
        private OrderItem() { }

        public OrderItem(
            Guid productId,
            Guid productVariantId,
            string productName,
            string productSlug,
            string? variantName,
            int quantity,
            decimal unitPrice,
            string? imageUrl)
        {
            ProductId = productId;
            ProductVariantId = productVariantId;
            ProductName = productName;
            ProductSlug = productSlug;
            VariantName = variantName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            ImageUrl = imageUrl;
            SetCreated();
        }
    }
}
