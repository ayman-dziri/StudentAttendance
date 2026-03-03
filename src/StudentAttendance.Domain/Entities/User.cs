using StudentAttendance.src.StudentAttendance.Domain.Enums;
using System.CodeDom.Compiler;

namespace StudentAttendance.src.StudentAttendance.Domain.Entities
{
    public class User
    {

        public string Id { get; set; } = null!;
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public DateOnly BirthDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Role Role { get; set; }

        public bool IsActive { get; set; } = true;

        public string? GroupId { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
        public DateTime? RefreshTokenRevokedAt { get; set; }

        public void GenerateEmail() // generer un email automatiquement en concatenant le nom + .prenom + .@Winity-artner.com
        {
            var domain = "@Winity-Partner.com";
            var UpperLastname = LastName.ToUpper();
            Email = $"{FirstName}.{UpperLastname}.{domain}";
        }

        public bool HasValidRefreshToken(string token, DateTime nowUtc)
        {
            if (RefreshToken is null || RefreshTokenExpiresAt is null) return false;
            if (RefreshTokenRevokedAt is not null) return false;
            if (!string.Equals(RefreshToken, token, StringComparison.Ordinal)) return false;

            return RefreshTokenExpiresAt.Value > nowUtc;
        }

        public void SetRefreshToken(string token, DateTime expiresAtUtc)
        {
            RefreshToken = token;
            RefreshTokenExpiresAt = expiresAtUtc;
            RefreshTokenRevokedAt = null;
        }

        public void RevokeRefreshToken(DateTime nowUtc)
        {
            RefreshTokenRevokedAt = nowUtc;
        }
    }
}
