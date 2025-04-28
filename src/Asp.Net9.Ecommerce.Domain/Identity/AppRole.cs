using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace Asp.Net9.Ecommerce.Domain.Identity
{
    public class AppRole : IdentityRole<Guid>
    {
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        
        // Protected constructor for EF Core
        protected AppRole() { }

        // Factory method for creating new roles
        public static AppRole Create(string name, string description, Guid? id = null, DateTime? createdAt = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be empty", nameof(name));

            return new AppRole
            {
                Id = id ?? Guid.NewGuid(),
                Name = name,
                Description = description,
                CreatedAt = createdAt ?? DateTime.UtcNow,
                NormalizedName = name.ToUpperInvariant()
            };
        }

        // Method to update role details
        public void Update(string description)
        {
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }
    }
} 