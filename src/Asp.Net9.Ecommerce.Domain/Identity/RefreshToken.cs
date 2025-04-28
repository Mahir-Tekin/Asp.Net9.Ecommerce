using Asp.Net9.Ecommerce.Domain.Common;
using System;
using System.Collections.Generic;

namespace Asp.Net9.Ecommerce.Domain.Identity
{
    public class RefreshToken : ValueObject
    {
        public string Token { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime ExpiresOnUtc { get; private set; }
        public DateTime? RevokedOnUtc { get; private set; }
        public string? ReplacedByToken { get; private set; }
        public string? ReasonRevoked { get; private set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOnUtc;
        public bool IsRevoked => RevokedOnUtc != null;
        public bool IsActive => !IsRevoked && !IsExpired;

        // Navigation property
        public AppUser User { get; private set; }

        protected RefreshToken() { } // For EF Core

        public static RefreshToken Create(string token, Guid userId, DateTime expiresOnUtc)
        {
            return new RefreshToken
            {
                Token = token,
                UserId = userId,
                ExpiresOnUtc = expiresOnUtc
            };
        }

        public void Revoke(string? replacedByToken = null, string? reason = null)
        {
            RevokedOnUtc = DateTime.UtcNow;
            ReplacedByToken = replacedByToken;
            ReasonRevoked = reason;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Token;
            yield return UserId;
            yield return ExpiresOnUtc;
            if (RevokedOnUtc.HasValue) yield return RevokedOnUtc.Value;
            if (ReplacedByToken != null) yield return ReplacedByToken;
            if (ReasonRevoked != null) yield return ReasonRevoked;
        }
    }
} 