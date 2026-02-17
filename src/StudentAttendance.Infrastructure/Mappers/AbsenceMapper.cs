using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Infrastructure.Documents;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Mappers
{
    public class AbsenceMapper
    {
        public static AbsenceDocument ToDocument(Absence u) => new()
        {
            Id = u.Id,
            Status = u.Status,
            StudentId = u.StudentId,
            SessionId = u.SessionId,
            JustificationDate = u.JustificationDate,
        };

        public static Absence ToDomain(AbsenceDocument d) => new()
        {
            Id = d.Id,
            Status = d.Status,
            StudentId = d.StudentId,
            SessionId = d.SessionId,
            JustificationDate = d.JustificationDate,
        };
    }
}
