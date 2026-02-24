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
            Password = c.Password,
            BirthDate = c.BirthDate,
            Role = c.Role,
        };

        public static CreateUserRequest ToCreateUserRequest(User u) => new()
        {
            FirstName = u.FirstName,
            LastName = u.LastName,
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
            BirthDate = up.BirthDate,
            GroupId = up.GroupId,
        };

        // ********************************************************

        public static UserDetailsResponse ToUserDetail(User u) => new()
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            BirthDate = u.BirthDate,
            CreatedAt = u.CreatedAt,
            Role = u.Role,
            IsActive = u.IsActive,
            GroupId = u.GroupId,
        };
    }
}
