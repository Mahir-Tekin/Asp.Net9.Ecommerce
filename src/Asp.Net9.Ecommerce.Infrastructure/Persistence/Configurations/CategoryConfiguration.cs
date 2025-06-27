using Asp.Net9.Ecommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asp.Net9.Ecommerce.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.Property(c => c.Slug)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(c => c.Slug)
                .IsUnique();

            // Self-referencing relationship
            builder.HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Many-to-many relationship with VariationType
            builder
                .HasMany(c => c.VariationTypes)
                .WithMany(vt => vt.Categories)
                .UsingEntity(j => j.ToTable("CategoryVariationTypes"));

            // Indexes
            builder.HasIndex(c => c.ParentCategoryId);
            builder.HasIndex(c => c.Name);
        }
    }
}