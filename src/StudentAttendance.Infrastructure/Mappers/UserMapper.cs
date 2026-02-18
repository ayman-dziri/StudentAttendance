using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Infrastructure.Documents;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Mappers
{
    public class UserMapper
    {
        public static UserDocument ToDocument(User u) => new()
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Password = u.Password,
            BirthDate = u.BirthDate,
            CreatedAt = u.CreatedAt,
            Role = u.Role,
            IsActive = u.IsActive,
            GroupId = u.GroupId,
        };

        public static User ToDomain(UserDocument d) => new()
        {
            Id = d.Id,
            FirstName = d.FirstName,
            LastName = d.LastName,
            Email = d.Email,
            Password = d.Password,
            BirthDate = d.BirthDate,
            CreatedAt = d.CreatedAt,
            Role = d.Role,
            IsActive = d.IsActive,
            GroupId = d.GroupId,
        };
    }
}
