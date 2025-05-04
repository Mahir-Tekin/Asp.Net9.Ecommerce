using System;

namespace Asp.Net9.Ecommerce.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; protected set; }
        public DateTime? DeletedAt { get; protected set; } 
        public bool IsDeleted => DeletedAt.HasValue;

        protected BaseEntity()
        {
            // Protected constructor to enforce creation through factory methods
        }

        // Methods for audit trail
        public void SetCreated()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetDeleted()
        {
            DeletedAt = DateTime.UtcNow;
        }

        public override bool Equals(object obj)
        {
            if (obj is not BaseEntity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            if (Id == Guid.Empty || other.Id == Guid.Empty)
                return false;

            return Id == other.Id;
        }

        public static bool operator ==(BaseEntity? a, BaseEntity? b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(BaseEntity? a, BaseEntity? b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().ToString() + Id).GetHashCode();
        }
    }
}
