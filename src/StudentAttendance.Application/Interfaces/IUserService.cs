using StudentAttendance.src.StudentAttendance.Application.DTOs.user;

namespace StudentAttendance.src.StudentAttendance.Application.Interfaces
{
    public interface IUserService
    {
        Task CreateUserAsync(CreateUserRequest request);
    }
}
