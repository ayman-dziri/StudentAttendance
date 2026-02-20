using StudentAttendance.src.StudentAttendance.Application.DTOs.user;
using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Application.Interfaces
{
    public interface IUserService
    {
        Task CreateUserAsync(CreateUserRequest userDto, CancellationToken ct = default);
        Task<User?> GetByIdAsync(string id, CancellationToken ct = default);
        Task<List<User>> GetAllUsersAsync(CancellationToken ct = default);
        Task<bool> UpdateUserAsync(UpdateUserRequest updateUser, CancellationToken ct = default);
        Task<bool> DeleteUserAsync(string id, CancellationToken ct = default);
    }
}
