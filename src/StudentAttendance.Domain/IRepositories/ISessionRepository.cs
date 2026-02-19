using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Domain.IRepositories
{
    public interface ISessionRepository
    {
        Task<Session?> GetByIdAsync(string id);
        Task UpdateAsync(Session session);
        Task ValidateAsync(string sessionId);

    }
}
