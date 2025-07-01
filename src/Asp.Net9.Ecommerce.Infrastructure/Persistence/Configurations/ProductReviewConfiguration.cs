using Asp.Net9.Ecommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asp.Net9.Ecommerce.Infrastructure.Persistence.Configurations
{
    public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            builder.HasKey(r => r.Id);

            // Basic Information
            builder.Property(r => r.ProductId)
                .IsRequired();

            builder.Property(r => r.UserId)
                .IsRequired();

            builder.Property(r => r.Rating)
                .IsRequired();

            builder.Property(r => r.Title)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(r => r.Comment)
                .HasMaxLength(2000)
                .IsRequired(false);

            // Helpfulness tracking
            builder.Property(r => r.HelpfulVotes)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(r => r.UnhelpfulVotes)
                .HasDefaultValue(0)
                .IsRequired();

            // Relationships
            builder.HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Note: UserId is just a Guid reference to User in Identity context
            // No foreign key constraint since User is in different DbContext

            // Constraints
            builder.ToTable(t => t.HasCheckConstraint("CK_ProductReviews_Rating", "Rating >= 1 AND Rating <= 5"));
            builder.ToTable(t => t.HasCheckConstraint("CK_ProductReviews_HelpfulVotes", "HelpfulVotes >= 0"));
            builder.ToTable(t => t.HasCheckConstraint("CK_ProductReviews_UnhelpfulVotes", "UnhelpfulVotes >= 0"));
            builder.ToTable(t => t.HasCheckConstraint("CK_ProductReviews_Content", "Title IS NOT NULL OR Comment IS NOT NULL"));

            // Indexes for performance
            builder.HasIndex(r => r.ProductId);
            builder.HasIndex(r => r.UserId);
            builder.HasIndex(r => r.Rating);
            builder.HasIndex(r => new { r.ProductId, r.UserId }).IsUnique(); // One review per user per product
            builder.HasIndex(r => new { r.ProductId, r.DeletedAt }); // For active reviews per product
            builder.HasIndex(r => r.CreatedAt); // For ordering reviews by date
        }
    }
}
