using Asp.Net9.Ecommerce.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asp.Net9.Ecommerce.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(oi => oi.ProductSlug)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(oi => oi.VariantName)
                .HasMaxLength(100);

            builder.Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            builder.Property(oi => oi.Quantity)
                .IsRequired();

            builder.Property(oi => oi.ImageUrl)
                .HasMaxLength(500);
        }
    }
}
