using Asp.Net9.Ecommerce.Domain.Catalog;
using Asp.Net9.Ecommerce.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Asp.Net9.Ecommerce.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<VariationType> VariationTypes { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Update audit fields before saving
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.SetCreated();
                        break;

                    case EntityState.Modified:
                        entry.Entity.SetUpdated();
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.SetDeleted();
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
} 