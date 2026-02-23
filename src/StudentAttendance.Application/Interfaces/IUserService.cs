using StudentAttendance.src.StudentAttendance.Application.DTOs.user;
using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Application.Interfaces
{
    public interface IUserService
    {
        Task CreateUserAsync(CreateUserRequest userDto, CancellationToken ct = default);
        Task<UserDetailsResponse?> GetByIdAsync(string id, CancellationToken ct = default);
        Task<List<UserDetailsResponse>> GetAllUsersAsync(CancellationToken ct = default);
        Task<bool> UpdateUserAsync(string id, UpdateUserRequest updateUser, CancellationToken ct = default);
        Task<bool> DeleteUserAsync(string id, CancellationToken ct = default);
        Task<User?> GetUserByEmail(string email, CancellationToken ct = default);
    }
}
