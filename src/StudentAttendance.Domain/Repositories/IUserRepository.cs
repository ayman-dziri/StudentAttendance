using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(User user, CancellationToken cancellationToken = default);
        Task<User?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<List<User>> GetUsersAsync(CancellationToken ct = default);
        Task<bool> UpdateUserAsync(string id, User user, CancellationToken ct = default);
        Task<bool> DeleteUserAsync(string id, CancellationToken ct = default);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken ct = default);
        Task<List<User>> GetStudentsByGroupIdAsync(string groupId, CancellationToken ct = default);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
        Task<bool> UpdateRefreshTokenAsync(
            string userId,
            string? refreshToken,
            DateTime? expiresAtUtc,
            DateTime? revokedAtUtc,
            CancellationToken ct = default
        );
        Task<bool> InvalidateAllAsync(string userId, CancellationToken ct = default);
    }
}
