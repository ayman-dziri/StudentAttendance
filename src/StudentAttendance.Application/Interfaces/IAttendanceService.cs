using StudentAttendance.src.StudentAttendance.Application.DTOs.Attendance;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Enums;
using StudentAttendance.src.StudentAttendance.Domain.IRepositories;

namespace StudentAttendance.src.StudentAttendance.Application.Interfaces
{
    public interface IAttendanceService
    {
        Task ValidateAndMarkAsync(string teacherId, string sessionId, MarkAbsencesRequest request);
        Task<List<AbsenceDto>> GetMyAbsencesAsync(string studentId);
    }

    public class AttendanceService : IAttendanceService
    {
        private readonly ISessionRepository _sessions;
        private readonly IAbsenceRepository _absences;

        public AttendanceService(ISessionRepository sessions, IAbsenceRepository absences)
        {
            _sessions = sessions;
            _absences = absences;
        }

        public async Task ValidateAndMarkAsync(string teacherId, string sessionId, MarkAbsencesRequest request)
        {
            var session = await _sessions.GetByIdAsync(sessionId)
                ?? throw new Exception("Session not found");

            if (session.TeacherId != teacherId)
                throw new Exception("Unauthorized");

            // Upsert absences
            foreach (var mark in request.Marks)
            {
                var existing = await _absences.GetByStudentAndSessionAsync(mark.StudentId, sessionId);

                if (existing is null)
                {
                    await _absences.CreateAsync(new Absence
                    {
                        Id = Guid.NewGuid().ToString(),
                        StudentId = mark.StudentId,
                        SessionId = sessionId,
                        Status = mark.Status,
                        JustificationDate = mark.JustificationDate
                    });
                }
                else
                {
                    existing.Status = mark.Status;
                    existing.JustificationDate = mark.JustificationDate;
                    await _absences.UpdateAsync(existing);
                }
            }

            //  Validation session (si tu l'as déjà ajouté)
            await _sessions.ValidateAsync(sessionId);
        }

        public async Task<List<AbsenceDto>> GetMyAbsencesAsync(string studentId)
        {
            var absences = await _absences.GetByStudentIdAsync(studentId);

            //  Ici : filtrer seulement Absent / Retard / Justifié
            // Option 1 (si ton enum a PRESENT): exclure PRESENT
            // var filtered = absences.Where(a => a.Status != StatusPresence.PRESENT);

            // Option 2 (recommandée) : explicite
            var filtered = absences.Where(a =>
                a.Status == StatusPresence.ABSENT ||
                a.Status == StatusPresence.LATE ||
                a.Status == StatusPresence.JUSTIFIED
            );

            // Mapping record AbsenceDto (constructeur)
            return filtered
                .Select(a => new AbsenceDto(
                    a.Id,
                    a.SessionId,
                    a.Status,
                    a.JustificationDate
                ))
                .ToList();
        }
    }
}