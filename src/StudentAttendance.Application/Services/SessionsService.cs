using FluentValidation;
using StudentAttendance.src.StudentAttendance.Application.Intefaces;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using System.Security.Cryptography.X509Certificates;

namespace StudentAttendance.src.StudentAttendance.Application.Services;

    public class SessionsService : ISessionsService
    {
    private readonly ISessionsRepository _sessionsRepository;
    private readonly ILogger<SessionsService> _logger;
    private readonly IValidator<Session> _validator;


    public async Task<List<Session>> GetAllSessionsAsync() {
        try
        {
            _logger.LogInformation("Getting all sessions");
            return await _sessionsRepository.GetAllSessionsAsync();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error getting all sessions");
            throw;
        }
    }

    public async Task<Session?> GetSessionsByIdAsync(string id)
    {
        try
        {
            _logger.LogInformation("Getting session by id: {SessionIdId}", id);
            return await _sessionsRepository.GetSessionsByIdAsync(id);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting session by id: {SessionId}", id);
            throw;
        }
    }

        public async Task<List<Session>> GetSessionsByTeacherIdAsync(string teacherId)
        {
            try
            {
                _logger.LogInformation("Getting sessions by teacher id: {TeacherId}", teacherId);
                return await _sessionsRepository.GetSessionsByTeacherIdAsync(teacherId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions by teacher id: {TeacherId}", teacherId);
                throw;
            }
    }

    public async Task<List<Session>> GetSessionsByGroupName(string group)
    {
        try
        {
            _logger.LogInformation("Getting sessions by group name: {GroupName}", group);
            return await _sessionsRepository.GetSessionsByGroupName(group);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sessions by group name: {GroupName}", group);
            throw;
        }
    }
   
    public async Task<Session> CreateSessionsAsync(Session session)
    {
        try
        {
            _logger.LogInformation("Creating session");

            var validationResult = await _validator.ValidateAsync(session);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult .Errors);
            }
            else
            {
                return await _sessionsRepository.CreateSessionsAsync(session);
            }
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating session");
            throw;
        }
    }

    public async Task<Session> UpdateSessionsAsync(string id, Session session)
    {
        try
        {
            _logger.LogInformation("Updating order with Id : {OrderId}", id);

            var existingSession = await _sessionsRepository.GetSessionsByIdAsync(id);
            if (existingSession == null)
            {
                _logger.LogWarning("Session with ID : {SessionId} not found", id);
                return null;
            }
            else
            {
                var validationresult = await _validator.ValidateAsync(session);

                if (!validationresult.IsValid)
                {
                    throw new ValidationException(validationresult.Errors);
                }
                else
                {
                    session.Id = id;
                    var updated = await _sessionsRepository.UpdateSessionsAsync(id, session);

                    return updated ? session : null;
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

                var existingSession = await _sessionsRepository.GetSessionsByIdAsync(id);

                if (existingSession == null)
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

