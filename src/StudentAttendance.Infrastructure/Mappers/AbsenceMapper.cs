using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Infrastructure.Documents;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Mappers
{
    public class AbsenceMapper
    {
        public static AbsenceDocument ToDocument(Absence u) => new()
        {
            Id = u.Id,

        };

        public static Absence ToDomain(AbsenceDocument d) => new()
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
