using Asp.Net9.Ecommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asp.Net9.Ecommerce.Infrastructure.Persistence.Configurations
{
    public class VariantOptionConfiguration : IEntityTypeConfiguration<VariantOption>
    {
        public void Configure(EntityTypeBuilder<VariantOption> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Value)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(o => o.DisplayValue)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(o => o.SortOrder)
                .IsRequired();

            // Optional: Add index for fast lookup
            builder.HasIndex(o => o.Value);

        }
    }
}
