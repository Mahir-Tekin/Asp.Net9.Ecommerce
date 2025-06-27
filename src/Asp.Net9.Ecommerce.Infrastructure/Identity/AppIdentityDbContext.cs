using Asp.Net9.Ecommerce.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Asp.Net9.Ecommerce.Infrastructure.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize Identity tables names
            builder.Entity<AppUser>().ToTable("Users");
            builder.Entity<AppRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

            // Configure AppUser
            builder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);

                // Configure RefreshTokens as owned entity
                entity.OwnsMany(e => e.RefreshTokens, rt =>
                {
                    rt.ToTable("UserRefreshTokens");
                    rt.WithOwner(t => t.User).HasForeignKey(t => t.UserId);
                    rt.HasKey(t => new { t.UserId, t.Token });
                    rt.Property(t => t.Token).IsRequired();
                    rt.Property(t => t.ExpiresOnUtc).IsRequired();
                });

                // Configure Addresses as one-to-many
                entity.HasMany(e => e.Addresses)
                      .WithOne()
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Address>().ToTable("UserAddresses");

            // Configure AppRole
            builder.Entity<AppRole>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            // Seed default roles
            builder.Entity<AppRole>().HasData(AppRoles.DefaultRoles);
        }
    }
}