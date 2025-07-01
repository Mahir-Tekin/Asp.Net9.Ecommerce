using Asp.Net9.Ecommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asp.Net9.Ecommerce.Infrastructure.Persistence.Configurations
{
    public class ReviewVoteConfiguration : IEntityTypeConfiguration<ReviewVote>
    {
        public void Configure(EntityTypeBuilder<ReviewVote> builder)
        {
            builder.ToTable("ReviewVotes");

            // Primary key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.ReviewId)
                .IsRequired();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.VoteType)
                .IsRequired()
                .HasConversion<int>(); // Store enum as int

            // Unique constraint - one vote per user per review
            builder.HasIndex(x => new { x.ReviewId, x.UserId })
                .IsUnique()
                .HasDatabaseName("IX_ReviewVotes_ReviewId_UserId");

            // Relationships
            builder.HasOne(x => x.Review)
                .WithMany(x => x.Votes)
                .HasForeignKey(x => x.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            // No foreign key to Users table since it's in different context
            // UserId is just a Guid reference
        }
    }
}
