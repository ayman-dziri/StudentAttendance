using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.IRepositories;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Repositories.Mocks
{
    public class FakeSessionRepository : ISessionRepository
    {
        private readonly List<Session> _sessions = new()
        {
            new Session
            {
                Id = "SESSION_1",
                TeacherId = "TEACHER_1",
                Group = "GROUP_1",
                StartTime = DateTime.UtcNow.AddHours(-2),
                EndTime = DateTime.UtcNow.AddHours(-1),
            }
        };

        public Task<Session?> GetByIdAsync(string id)
            => Task.FromResult(_sessions.FirstOrDefault(s => s.Id == id));

        public Task UpdateAsync(Session session)
        {
            var idx = _sessions.FindIndex(s => s.Id == session.Id);
            if (idx >= 0) _sessions[idx] = session;
            return Task.CompletedTask;
        }
        public Task ValidateAsync(string sessionId)
        {
            var s = _sessions.FirstOrDefault(x => x.Id == sessionId);
            if (s is null) return Task.CompletedTask;

            s.IsValidated = true;
            return Task.CompletedTask;
        }

    }
}
