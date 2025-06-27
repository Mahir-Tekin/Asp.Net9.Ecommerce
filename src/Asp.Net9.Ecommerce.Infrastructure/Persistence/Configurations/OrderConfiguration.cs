using Asp.Net9.Ecommerce.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asp.Net9.Ecommerce.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            builder.OwnsOne(o => o.ShippingAddress, sa =>
            {
                sa.Property(a => a.FirstName).IsRequired().HasMaxLength(100);
                sa.Property(a => a.LastName).IsRequired().HasMaxLength(100);
                sa.Property(a => a.PhoneNumber).IsRequired().HasMaxLength(15);
                sa.Property(a => a.City).IsRequired().HasMaxLength(100);
                sa.Property(a => a.District).IsRequired().HasMaxLength(100);
                sa.Property(a => a.Neighborhood).IsRequired().HasMaxLength(100);
                sa.Property(a => a.AddressLine).IsRequired().HasMaxLength(200);
                sa.Property(a => a.AddressTitle).IsRequired().HasMaxLength(50);
            });

            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
