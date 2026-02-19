using StudentAttendance.src.StudentAttendance.Application.DTOs.user;
using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Application.Mappers
{
    public class UserMapper
    {
        public static User ToEntity(CreateUserRequest request) => new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = request.Password,
            BirthDate = request.BirthDate,
            Role = request.Role,
            GroupId = request.GroupId,
        };

        public static CreateUserRequest TocreateUserRequest(User user) => new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Password = user.Password,
            BirthDate = user.BirthDate,
            Role = user.Role,
            GroupId = user.GroupId,
        };
    }
}
