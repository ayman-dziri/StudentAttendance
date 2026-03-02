using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.IRepositories;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Attendance;

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

            if (session.IsValidated)
                throw new Exception("Session already validated");

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

            await _sessions.ValidateAsync(sessionId); //pour changer l'état d'invalide à valide
        }


        public async Task<List<AbsenceDto>> GetMyAbsencesAsync(string studentId)
        {
            var absences = await _absences.GetByStudentIdAsync(studentId);

            return absences.Select(a => new AbsenceDto(
                a.Id,
                a.SessionId,
                a.Status,
                a.JustificationDate
            )).ToList();
        }
    }
}
