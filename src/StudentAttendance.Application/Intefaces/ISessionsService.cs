using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Application.Intefaces;

    public interface ISessionsService
    {
    Task<List<Session>> GetAllSessionsAsync();
    Task<Session?> GetSessionsByIdAsync(string id);
    Task<List<Session>> GetSessionsByTeacherIdAsync(string teacherId);

    Task<List<Session>> GetSessionsByGroupName(string group);
    Task<Session> CreateSessionsAsync(Session session);
    Task<Session> UpdateSessionsAsync(string id, Session session);
    Task<bool> DeleteSessionsAsync(string id);



}

