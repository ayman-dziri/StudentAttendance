using FluentValidation;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Requests;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Response;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Application.Mappers;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Enums;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;

namespace StudentAttendance.src.StudentAttendance.Application.Services;

public class SessionsService : ISessionsService
{
    private readonly ISessionsRepository _sessionsRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    private readonly ILogger<SessionsService> _logger;
    private readonly IValidator<CreateSessionRequest> _createValidator;
    private readonly IValidator<UpdateSessionRequest> _updateValidator;
    private readonly ISessionConflictValidator _sessionConflictValidator;

    public SessionsService(
        ISessionsRepository sessionsRepository,
        IGroupRepository groupRepository,
        IUserRepository userRepository,
        ILogger<SessionsService> logger,
        IValidator<CreateSessionRequest> createValidator,
        IValidator<UpdateSessionRequest> updateValidator,
        ISessionConflictValidator sessionConflictValidator)
    {
        _sessionsRepository = sessionsRepository;
        _groupRepository = groupRepository;
        _userRepository = userRepository;

        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _sessionConflictValidator = sessionConflictValidator;
    }

    public async Task<List<SessionResponse>> GetAllSessionsAsync()
    {
        try
        {
            _logger.LogInformation("Getting all sessions");

            var sessions = await _sessionsRepository.GetAllSessionsAsync();
            return sessions.Select(SessionMapper.ToResponse).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all sessions");
            throw;
        }
    }

    public async Task<SessionResponse?> GetSessionsByIdAsync(string id)
    {
        try
        {
            _logger.LogInformation("Getting session by id: {SessionId}", id);

            var session = await _sessionsRepository.GetSessionsByIdAsync(id);
            return session is null ? null : SessionMapper.ToResponse(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting session by id: {SessionId}", id);
            throw;
        }
    }

    public async Task<List<SessionResponse>> GetSessionsByTeacherIdAsync(string teacherId)
    {
        try
        {
            _logger.LogInformation("Getting sessions by teacher id: {TeacherId}", teacherId);

            var sessions = await _sessionsRepository.GetSessionsByTeacherIdAsync(teacherId);
            return sessions.Select(SessionMapper.ToResponse).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sessions by teacher id: {TeacherId}", teacherId);
            throw;
        }
    }

    public async Task<List<SessionResponse>> GetSessionsByGroupName(string group)
    {
        try
        {
            _logger.LogInformation("Getting sessions by group name: {GroupName}", group);

            var sessions = await _sessionsRepository.GetSessionsByGroupName(group);
            return sessions.Select(SessionMapper.ToResponse).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sessions by group name: {GroupName}", group);
            throw;
        }
    }

    public async Task<List<User>> GetStudentsBySessionIdAsync(string sessionId)
    {
        try
        {
            _logger.LogInformation("Getting students by session id: {SessionId}", sessionId);

            
            return await _sessionsRepository.GetStudentsBySessionIdAsync(sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting students by session id: {SessionId}", sessionId);
            throw;
        }
    }

    public async Task<string?> GetProfessurBySessionIdAsync(string sessionId)
    {
        try
        {
            _logger.LogInformation("Getting professor for session {SessionId}", sessionId);
            return await _sessionsRepository.GetProfessurBySessionIdAsync(sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting professor for session {SessionId}", sessionId);
            throw;
        }
    }

    public async Task<SessionResponse> CreateSessionsAsync(
        CreateSessionRequest sessionrequest,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating session");

            var validationResult = await _createValidator.ValidateAsync(sessionrequest, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var session = SessionMapper.ToEntity(sessionrequest);

            await _sessionConflictValidator.ValidateNoConflictAsync(session, null, cancellationToken);

            // Recuperer le Group par label = nom
            var group = await _groupRepository.GetByNameAsync(session.Group, cancellationToken);
            if (group is null)
                throw new Exception($"Group '{session.Group}' not found.");

            // Recuperer les étudiants via GroupId
            var students = await _userRepository.GetStudentsByGroupIdAsync(group.Id, cancellationToken);

            // Creer les absences 
            session.Absences = students
                .Select(s => new Absence
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    StudentId = s.Id,
                    Status = StatusPresence.PRESENT,
                    JustificationDate = null
                })
                .ToList();

            var created = await _sessionsRepository.CreateSessionsAsync(session);

            return SessionMapper.ToResponse(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating session");
            throw;
        }
    }

    public async Task<SessionResponse?> UpdateSessionsAsync(string id, UpdateSessionRequest sessionrequest)
    {
        try
        {
            _logger.LogInformation("Updating session with Id: {SessionId}", id);

            var existingSession = await _sessionsRepository.GetSessionsByIdAsync(id);
            if (existingSession is null)
            {
                _logger.LogWarning("Session with ID: {SessionId} not found", id);
                return null;
            }

            var validationResult = await _updateValidator.ValidateAsync(sessionrequest);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Garder lancien group pour detecter un changement
            var oldGroupLabel = existingSession.Group;

            // Appliquer update sur lentite existante
            SessionMapper.MapUpdate(sessionrequest, existingSession);

            // Vérifier conflit de planning
            await _sessionConflictValidator.ValidateNoConflictAsync(existingSession, id, CancellationToken.None);

            // Si le groupe a changé, on régénère la liste des absences
            if (!string.Equals(oldGroupLabel, existingSession.Group, StringComparison.OrdinalIgnoreCase))
            {
                var group = await _groupRepository.GetByNameAsync(existingSession.Group, CancellationToken.None);
                if (group is null)
                    throw new Exception($"Group '{existingSession.Group}' not found.");

                var students = await _userRepository.GetStudentsByGroupIdAsync(group.Id, CancellationToken.None);

                existingSession.Absences = students
                    .Select(s => new Absence
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        StudentId = s.Id,
                        Status = StatusPresence.PRESENT,
                        JustificationDate = null
                    })
                    .ToList();
            }

            var updated = await _sessionsRepository.UpdateSessionsAsync(id, existingSession);
            if (!updated) return null;

            return SessionMapper.ToResponse(existingSession);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating session with ID: {SessionId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteSessionsAsync(string id)
    {
        try
        {
            _logger.LogInformation("Deleting session with ID: {SessionId}", id);

            var exists = await _sessionsRepository.ExistsSessionAsync(id);
            if (!exists)
            {
                _logger.LogWarning("Session with ID: {SessionId} not found", id);
                return false;
            }

            return await _sessionsRepository.DeleteSessionsAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting session with ID: {SessionId}", id);
            throw;
        }
    }
}