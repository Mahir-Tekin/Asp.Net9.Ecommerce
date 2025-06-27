using Asp.Net9.Ecommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Asp.Net9.Ecommerce.Infrastructure.Persistence.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.HasKey(v => v.Id);

            // Basic information
            builder.Property(v => v.SKU)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(v => v.Name)
                .HasMaxLength(200)
                .IsRequired();

            // Pricing
            builder.Property("_price")
                .HasColumnName("Price")
                .HasPrecision(18, 2);
            builder.Property(v => v.OldPrice)
                .HasPrecision(18, 2);

            // Add check constraint for Price
            builder.ToTable(t => t.HasCheckConstraint("CK_ProductVariants_Price", "Price IS NULL OR Price > 0"));

            // Inventory
            builder.Property("_stockQuantity")
                .HasColumnName("StockQuantity")
                .IsRequired();

            // Add check constraint for StockQuantity
            builder.ToTable(t => t.HasCheckConstraint("CK_ProductVariants_StockQuantity", "StockQuantity >= 0"));

            builder.Property(v => v.MinStockThreshold)
                .IsRequired();

            // Add check constraint for MinStockThreshold
            builder.ToTable(t => t.HasCheckConstraint("CK_ProductVariants_MinStockThreshold", "MinStockThreshold >= 0"));

            builder.Property(v => v.IsActive)
                .IsRequired();

            builder.Property(v => v.TrackInventory)
                .IsRequired();


            // Many-to-many: ProductVariant <-> VariantOption
            builder
                .HasMany(v => v.SelectedOptions)
                .WithMany()
                .UsingEntity(j => j.ToTable("ProductVariantOptions"));

            // Improved Indexes
            builder.HasIndex(v => v.SKU).IsUnique().HasFilter("DeletedAt IS NULL"); // Unique SKU only for active records
            builder.HasIndex(v => v.ProductId);
            builder.HasIndex(v => v.IsActive);
            builder.HasIndex("_stockQuantity"); // For inventory queries
            builder.HasIndex(v => new { v.IsActive, v.DeletedAt }); // Composite index for active variants
            builder.HasIndex(v => new { v.ProductId, v.IsActive }); // For querying active variants of a product

            builder.Property(v => v.RowVersion)
                .IsRowVersion();
        }
    }
}