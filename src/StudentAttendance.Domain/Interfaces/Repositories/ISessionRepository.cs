namespace StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;

using global::StudentAttendance.src.StudentAttendance.Domain.Entities;




    public interface ISessionRepository
    {
        Task<Session?> GetByIdAsync(string id);
        Task UpdateAsync(Session session);
        Task ValidateAsync(string sessionId);

    }


