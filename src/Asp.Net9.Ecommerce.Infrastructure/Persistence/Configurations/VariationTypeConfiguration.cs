using Asp.Net9.Ecommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asp.Net9.Ecommerce.Infrastructure.Persistence.Configurations
{
    public class VariationTypeConfiguration : IEntityTypeConfiguration<VariationType>
    {
        public void Configure(EntityTypeBuilder<VariationType> builder)
        {
            builder.HasKey(t => t.Id);

            // Basic information
            builder.Property(t => t.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(t => t.DisplayName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.IsActive)
                .IsRequired();

            // Store options as JSON
            builder.OwnsMany(t => t.Options, options =>
            {
                options.ToJson();
            });

            // Indexes
            builder.HasIndex(t => t.Name).IsUnique();
            builder.HasIndex(t => t.IsActive);
        }
    }
} 