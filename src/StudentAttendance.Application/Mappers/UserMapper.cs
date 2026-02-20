using StudentAttendance.src.StudentAttendance.Application.DTOs.user;
using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Application.Mappers
{
    public class UserMapper
    {
        public static User ToEntity(CreateUserRequest c) => new()
        {
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            Password = c.Password,
            BirthDate = c.BirthDate,
            Role = c.Role,
            GroupId = c.GroupId,
        };

        public static CreateUserRequest TocreateUserRequest(User u) => new()
        {
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Password = u.Password,
            BirthDate = u.BirthDate,
            Role = u.Role,
            GroupId = u.GroupId,
        };

        // *******************************************************

        public static User ToEntity(UpdateUserRequest up) => new()
        {
            FirstName = up.FirstName,
            LastName = up.LastName,
            Email = up.Email,
            BirthDate = up.BirthDate,
            GroupId = up.GroupId,
        };
    }
}
