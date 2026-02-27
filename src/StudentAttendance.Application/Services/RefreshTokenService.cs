using System.ComponentModel;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
using StudentAttendanceV2.src.StudentAttendance.Application.Interfaces;

namespace StudentAttendanceV2.src.StudentAttendance.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {

        private readonly MongoDbContext _context;


        public RefreshTokenService(MongoDbContext context)
        {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task InvalidateAllAsync(string userId , CancellationToken ct = default )
        {
           if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("UserId cannot be null or empty.", nameof(userId));
        }
        }
    }
}