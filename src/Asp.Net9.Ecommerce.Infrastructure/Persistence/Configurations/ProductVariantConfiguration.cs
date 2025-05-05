using Asp.Net9.Ecommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
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

            // Inventory
            builder.Property("_stockQuantity")
                .HasColumnName("StockQuantity")
                .IsRequired();

            builder.Property(v => v.MinStockThreshold)
                .IsRequired();

            builder.Property(v => v.IsActive)
                .IsRequired();

            builder.Property(v => v.TrackInventory)
                .IsRequired();

            // Store variations as JSON
            var jsonOptions = new JsonSerializerOptions();
            var converter = new ValueConverter<Dictionary<string, string>, string>(
                v => JsonSerializer.Serialize(v, jsonOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, jsonOptions)
            );

            builder.Property("_variations")
                .HasColumnName("Variations")
                .HasColumnType("nvarchar(max)")
                .HasConversion(converter);

            // Indexes
            builder.HasIndex(v => v.SKU).IsUnique();
            builder.HasIndex(v => v.ProductId);
            builder.HasIndex(v => v.IsActive);
        }
    }
} 