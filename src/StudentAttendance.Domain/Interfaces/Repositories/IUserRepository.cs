using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;

    public interface IUserRepository
    {

    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<List<User>> GetStudentsByGroupIdAsync(string groupId, CancellationToken ct = default);

    Task<User> CreateUserAsync(User user);
}


