using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Infrastructure.Documents;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Mappers
{
    public class SessionMapper
    {
        // Domain → Mongo
        public static SessionDocument ToDocument(Session u) => new()
        {
            Id = u.Id,
            StartTime = u.StartTime,
            EndTime = u.EndTime,
            TeacherId = u.TeacherId,
            Group = u.Group,
            IsValidated = u.IsValidated,

            Absences = u.Absences?
                .Select(a => new AbsenceDocument
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    Status = a.Status,
                    JustificationDate = a.JustificationDate
                })
                .ToList() ?? new List<AbsenceDocument>()
        };

        // Mongo → Domain
        public static Session ToDomain(SessionDocument d) => new()
        {
            Id = d.Id,
            StartTime = d.StartTime,
            EndTime = d.EndTime,
            TeacherId = d.TeacherId,
            Group = d.Group,
            IsValidated = d.IsValidated,

            Absences = d.Absences?
                .Select(a => new Absence
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    Status = a.Status,
                    JustificationDate = a.JustificationDate
                })
                .ToList() ?? new List<Absence>()
        };
    }
}