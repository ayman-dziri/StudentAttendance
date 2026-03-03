using System.Security.Cryptography;
using StudentAttendance.src.StudentAttendance.Domain.Auth;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Auth
{
    public sealed class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        public string Generate()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }
    }
}