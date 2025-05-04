using Asp.Net9.Ecommerce.Domain.Common;

namespace Asp.Net9.Ecommerce.Domain.Catalog
{
    public class ProductImage : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public string Url { get; private set; }
        public string AltText { get; private set; }
        public int DisplayOrder { get; private set; }
        public bool IsMain { get; private set; }

        // Navigation property
        public Product Product { get; private set; }

        protected ProductImage() { } // For EF Core

        public static ProductImage Create(Guid productId, string url, string altText, int displayOrder, bool isMain)
        {
            return new ProductImage
            {
                ProductId = productId,
                Url = url,
                AltText = altText ?? string.Empty,
                DisplayOrder = displayOrder,
                IsMain = isMain
            };
        }

        public void UpdateDisplayOrder(int newOrder)
        {
            DisplayOrder = newOrder;
        }

        public void SetMain()
        {
            IsMain = true;
        }

        public void UnsetMain()
        {
            IsMain = false;
        }
    }
} 