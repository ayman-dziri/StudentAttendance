using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Infrastructure.Documents;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Mappers
{
    public class SessionMapper
    {
        public static SessionDocument ToDocument(Session u) => new()
        {
            Id = u.Id,
            StartTime = u.StartTime,
            EndTime = u.EndTime,
            TeacherId = u.TeacherId,
            Group = u.Group,
        };

        public static Session ToDomain(SessionDocument d) => new()
        {
            Id = d.Id,
            StartTime = d.StartTime,
            EndTime = d.EndTime,
            TeacherId = d.TeacherId,
            Group = d.Group,
        };
    }
}
