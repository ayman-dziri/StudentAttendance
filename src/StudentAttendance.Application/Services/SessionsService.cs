using FluentValidation;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Requests;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Response;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Application.Mappers;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;


namespace StudentAttendance.src.StudentAttendance.Application.Services;

public class SessionsService : ISessionsService
{
    //Service Absence
    //------------
    private readonly IAbsenceService _absenceService;
    //------------


    private readonly ISessionsRepository _sessionsRepository;
    private readonly ILogger<SessionsService> _logger;
    private readonly IValidator<CreateSessionRequest> _createValidator;
    private readonly IValidator<UpdateSessionRequest> _updateValidator;


    public SessionsService(
        IAbsenceService absenceService,
        ISessionsRepository sessionsRepository,
        ILogger<SessionsService> logger,
        IValidator<CreateSessionRequest> createValidator,
        IValidator<UpdateSessionRequest> updateValidator)
    {
        _absenceService = absenceService;
        _sessionsRepository = sessionsRepository;
        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
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
            _logger.LogInformation("Getting session by id: {SessionIdId}", id);
            var sessionbyid =  await _sessionsRepository.GetSessionsByIdAsync(id);
            return sessionbyid == null ? null : SessionMapper.ToResponse(sessionbyid);

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
            var sessionbyteacher =  await _sessionsRepository.GetSessionsByTeacherIdAsync(teacherId);
            return sessionbyteacher.Select(SessionMapper.ToResponse).ToList();
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
            var sessionbygroup = await _sessionsRepository.GetSessionsByGroupName(group);
            return sessionbygroup.Select(SessionMapper.ToResponse).ToList();
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
           //var students = 
                return await _sessionsRepository.GetStudentsBySessionIdAsync(sessionId);


            //return students.Select(UserMapper.ToResponse).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting students by session id: {SessionId}", sessionId);
            throw;
        }
    }

    public async Task<User?> GetProfessurBySessionIdAsync(string sessionId)
    {
        try
        {
            _logger.LogInformation("Getting professor for session {SessionId}", sessionId);

            //var teacher =
            return await _sessionsRepository.GetProfessurBySessionIdAsync(sessionId);

            // return teacher == null ? null : UserMapper.ToResponse(teacher);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting professor for session {SessionId}", sessionId);
            throw;
        }
    }



    public async Task<SessionResponse> CreateSessionsAsync(CreateSessionRequest sessionrequest, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating session");

            var validationResult = await _createValidator.ValidateAsync(sessionrequest , cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            else
            {
                var session = SessionMapper.ToEntity(sessionrequest);
                var created =  await _sessionsRepository.CreateSessionsAsync(session);

                // Get students of that session group 
                var students = await _sessionsRepository.GetStudentsBySessionIdAsync(created.Id);
                var studentIds = students.Select(student => student.Id).ToList();



                //implementation de creation de absence PRESENT pour les etudiants
                await _absenceService.CreateAbsencesForSessionAsync(created.Id, studentIds, cancellationToken);

                return SessionMapper.ToResponse(created);
            }

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
            _logger.LogInformation("Updating session with Id : {SessionId}", id);

            var existingSession = await _sessionsRepository.GetSessionsByIdAsync(id);
            if (existingSession == null)
            {
                _logger.LogWarning("Session with ID : {SessionId} not found", id);
                return null;
            }
            else
            {
                var validationresult = await _updateValidator.ValidateAsync(sessionrequest);

                if (!validationresult.IsValid)
                {
                    throw new ValidationException(validationresult.Errors);
                }
                else
                {
                    SessionMapper.MapUpdate(sessionrequest , existingSession);

                    var updated = await _sessionsRepository.UpdateSessionsAsync(id, existingSession);

                    return updated ? SessionMapper.ToResponse(existingSession) : null;
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating session with ID : {SessionId}", id);
            throw;
        }
    }


    public async Task<bool> DeleteSessionsAsync(string id)
    {
        try
        {
            _logger.LogInformation("Deleting session with ID : {SessionId}", id);

            var existingSession = await _sessionsRepository.ExistsSessionAsync(id);

            if (!existingSession)
            {
                _logger.LogWarning("Session with ID : {SessionId} not found", id);
                return false;
            }
            else
            {
                return await _sessionsRepository.DeleteSessionsAsync(id);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting session with ID : {SessionId}", id);
            throw;
        }
    }











}

