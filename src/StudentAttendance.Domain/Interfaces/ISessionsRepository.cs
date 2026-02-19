using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;

public interface ISessionsRepository
{
    Task<List<Session>> GetAllSessionsAsync();
    Task<Session?> GetSessionsByIdAsync(string id);
    Task<List<Session>> GetSessionsByTeacherIdAsync(string teacherId);
    Task<List<User>> GetStudentsBySessionIdAsync(string sessionId);
    Task<User?> GetProfessurBySessionIdAsync(string sessionId);
    Task<List<Session>> GetSessionsByGroupName(string group);
    Task<Session> CreateSessionsAsync(Session session);
    Task<bool> UpdateSessionsAsync(string id, Session session);
    Task<bool> DeleteSessionsAsync(string id);
    Task<bool> ExistsSessionAsync(string id);
}