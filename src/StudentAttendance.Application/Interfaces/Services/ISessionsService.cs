using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Requests;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Response;
using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;

public interface ISessionsService
{
    Task<List<SessionResponse>> GetAllSessionsAsync();
    Task<SessionResponse?> GetSessionsByIdAsync(string id);
    Task<List<SessionResponse>> GetSessionsByTeacherIdAsync(string teacherId);

    Task<List<SessionResponse>> GetSessionsByGroupName(string group);

    Task<SessionResponse> CreateSessionsAsync(CreateSessionRequest sessionrequest, CancellationToken cancellationToken = default);
    Task<SessionResponse> UpdateSessionsAsync(string id, UpdateSessionRequest sessionrequest);
    Task<bool> DeleteSessionsAsync(string id);


    Task<List<User>> GetStudentsBySessionIdAsync(string sessionId);
    Task<User?> GetProfessurBySessionIdAsync(string sessionId);





}

