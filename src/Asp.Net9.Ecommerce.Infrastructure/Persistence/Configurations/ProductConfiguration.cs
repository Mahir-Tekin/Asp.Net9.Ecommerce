using Asp.Net9.Ecommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asp.Net9.Ecommerce.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            // Basic information
            builder.Property(p => p.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(2000);

            // Pricing
            builder.Property(p => p.BasePrice)
                .HasPrecision(18, 2)
                .IsRequired();
            
            // Add check constraint for BasePrice
            builder.ToTable(t => t.HasCheckConstraint("CK_Products_BasePrice", "BasePrice > 0"));

            builder.Property(p => p.OldPrice)
                .HasPrecision(18, 2);

            // Status
            builder.Property(p => p.IsActive)
                .IsRequired();

            // Relationships
            builder.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Variants)
                .WithOne(v => v.Product)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.VariantTypes)
                .WithMany()
                .UsingEntity(j => j.ToTable("ProductVariantTypes"));

            // Store images as JSON
            builder.OwnsMany(p => p.Images, images =>
            {
                images.ToJson();
            });

            // Improved Indexes
            builder.HasIndex(p => p.Name);
            builder.HasIndex(p => p.CategoryId);
            builder.HasIndex(p => p.IsActive);
            builder.HasIndex(p => p.BasePrice); // Add index for price queries
            builder.HasIndex(p => new { p.IsActive, p.DeletedAt }); // Composite index for active products
        }
    }
} 