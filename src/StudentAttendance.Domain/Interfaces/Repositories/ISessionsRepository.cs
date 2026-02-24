using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Requests;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Enums;

namespace StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;

public interface ISessionsRepository
{
    Task<List<Session>> GetAllSessionsAsync();
    Task<Session?> GetSessionsByIdAsync(string id);
    Task<List<Session>> GetSessionsByTeacherIdAsync(string teacherId);

    Task<List<User>> GetStudentsBySessionIdAsync(string sessionId);
    Task<string?> GetProfessurBySessionIdAsync(string sessionId);
    Task<List<Session>> GetSessionsByGroupName(string group);
    Task<Session> CreateSessionsAsync(Session session);
    Task<bool> UpdateSessionsAsync(string id, Session session);
    Task<bool> DeleteSessionsAsync(string id);
    Task<bool> ExistsSessionAsync(string id);


    Task ValidateAsync(string sessionID, CancellationToken cancellationToken = default);

    Task<Session?> GetByIdAsync(string sessionId, CancellationToken cancellationToken = default);
    Task<bool> JustifyAbsenceAsync(string sessionId, string studentId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAbsenceStatusAsync(string sessionId, string studentId, StatusPresence status, CancellationToken cancellationToken = default);
    Task<bool> UpdateAbsencesBulkAsync(string sessionId, List<UpdateAbsencesBulkItem> items, CancellationToken cancellationToken = default);


    Task<List<Session>> GetSessionsWithStudentAbsenceAsync(string studentId, CancellationToken ct = default);




}

