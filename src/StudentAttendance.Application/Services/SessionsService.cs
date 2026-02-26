using FluentValidation;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Absence;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Requests;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Response;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Application.Mappers;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Enums;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Domain.Repositories;

namespace StudentAttendance.src.StudentAttendance.Application.Services;

/// <summary>
/// Opérations métier liées aux séances et aux absences embedded
/// </summary>
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

    /// <summary>
    /// Récupère toutes les séances
    /// </summary>
    public async Task<List<SessionResponse>> GetAllSessionsAsync()
    {
        try
        {
            _logger.LogInformation("Récupération de toutes les séances");
            var sessions = await _sessionsRepository.GetAllSessionsAsync();
            return sessions.Select(SessionMapper.ToResponse).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de toutes les séances");
            throw;
        }
    }

    /// <summary>
    /// Récupère une séance par son identifiant
    /// </summary>
    public async Task<SessionResponse?> GetSessionsByIdAsync(string id)
    {
        try
        {
            _logger.LogInformation("Récupération de la séance {SessionId}", id);
            var session = await _sessionsRepository.GetSessionsByIdAsync(id);
            return session is null ? null : SessionMapper.ToResponse(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de la séance {SessionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Récupère les séances d'un professeur
    /// </summary>
    public async Task<List<SessionResponse>> GetSessionsByTeacherIdAsync(string teacherId)
    {
        try
        {
            _logger.LogInformation("Récupération des séances du professeur {TeacherId}", teacherId);
            var sessions = await _sessionsRepository.GetSessionsByTeacherIdAsync(teacherId);
            return sessions.Select(SessionMapper.ToResponse).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des séances du professeur {TeacherId}", teacherId);
            throw;
        }
    }

    /// <summary>
    /// Récupère les séances d'un groupe
    /// </summary>
    public async Task<List<SessionResponse>> GetSessionsByGroupName(string group)
    {
        try
        {
            _logger.LogInformation("Récupération des séances du groupe {GroupName}", group);
            var sessions = await _sessionsRepository.GetSessionsByGroupName(group);
            return sessions.Select(SessionMapper.ToResponse).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des séances du groupe {GroupName}", group);
            throw;
        }
    }

    /// <summary>
    /// Récupère les étudiants d'une séance
    /// </summary>
    public async Task<List<User>> GetStudentsBySessionIdAsync(string sessionId)
    {
        try
        {
            _logger.LogInformation("Récupération des étudiants de la séance {SessionId}", sessionId);
            return await _sessionsRepository.GetStudentsBySessionIdAsync(sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des étudiants de la séance {SessionId}", sessionId);
            throw;
        }
    }

    /// <summary>
    /// Récupère le professeur d'une séance
    /// </summary>
    public async Task<string?> GetProfessurBySessionIdAsync(string sessionId)
    {
        try
        {
            _logger.LogInformation("Récupération du professeur de la séance {SessionId}", sessionId);
            return await _sessionsRepository.GetProfessurBySessionIdAsync(sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du professeur de la séance {SessionId}", sessionId);
            throw;
        }
    }

    /// <summary>
    /// Crée une nouvelle séance avec les absences embedded des étudiants du groupe
    /// </summary>
    public async Task<SessionResponse> CreateSessionsAsync(
        CreateSessionRequest sessionrequest,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Création d'une nouvelle séance");

            var validationResult = await _createValidator.ValidateAsync(sessionrequest, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var session = SessionMapper.ToEntity(sessionrequest);

            await _sessionConflictValidator.ValidateNoConflictAsync(session, null, cancellationToken);

            var group = await _groupRepository.GetGroupByLabelAsync(session.Group, cancellationToken);
            if (group is null)
                throw new KeyNotFoundException($"Le groupe '{session.Group}' est introuvable.");

            var students = await _userRepository.GetStudentsByGroupIdAsync(group.Id, cancellationToken);

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

            _logger.LogInformation("Séance {SessionId} créée avec {Count} absences", created.Id, created.Absences.Count);

            return SessionMapper.ToResponse(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création de la séance");
            throw;
        }
    }

    /// <summary>
    /// Met à jour une séance existante
    /// </summary>
    public async Task<SessionResponse?> UpdateSessionsAsync(string id, UpdateSessionRequest sessionrequest)
    {
        try
        {
            _logger.LogInformation("Mise à jour de la séance {SessionId}", id);

            var existingSession = await _sessionsRepository.GetSessionsByIdAsync(id);
            if (existingSession is null)
            {
                _logger.LogWarning("Séance {SessionId} introuvable", id);
                return null;
            }

            var validationResult = await _updateValidator.ValidateAsync(sessionrequest);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var oldGroupLabel = existingSession.Group;

            SessionMapper.MapUpdate(sessionrequest, existingSession);

            await _sessionConflictValidator.ValidateNoConflictAsync(existingSession, id, CancellationToken.None);

            if (!string.Equals(oldGroupLabel, existingSession.Group, StringComparison.OrdinalIgnoreCase))
            {
                var group = await _groupRepository.GetGroupByLabelAsync(existingSession.Group, CancellationToken.None);
                if (group is null)
                    throw new KeyNotFoundException($"Le groupe '{existingSession.Group}' est introuvable.");

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
            _logger.LogError(ex, "Erreur lors de la mise à jour de la séance {SessionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Supprime une séance
    /// </summary>
    public async Task<bool> DeleteSessionsAsync(string id)
    {
        try
        {
            _logger.LogInformation("Suppression de la séance {SessionId}", id);

            var exists = await _sessionsRepository.ExistsSessionAsync(id);
            if (!exists)
            {
                _logger.LogWarning("Séance {SessionId} introuvable", id);
                return false;
            }

            return await _sessionsRepository.DeleteSessionsAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression de la séance {SessionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Justifie une absence d'un étudiant dans une séance
    /// La séance doit être validée et le statut doit être ABSENT ou LATE
    /// </summary>
    public async Task<bool> JustifyAbsenceAsync(string sessionId, string studentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Justification de l'absence de l'étudiant {StudentId} dans la séance {SessionId}", studentId, sessionId);

            var session = await _sessionsRepository.GetSessionsByIdAsync(sessionId)
                ?? throw new KeyNotFoundException($"La séance '{sessionId}' est introuvable.");

            if (!session.IsValidated)
                throw new InvalidOperationException("Impossible de justifier une absence dans une séance non validée.");

            var absence = session.Absences.FirstOrDefault(a => a.StudentId == studentId)
                ?? throw new KeyNotFoundException($"Aucune absence trouvée pour l'étudiant '{studentId}' dans la séance '{sessionId}'.");

            if (absence.Status != StatusPresence.ABSENT && absence.Status != StatusPresence.LATE)
                throw new InvalidOperationException($"Impossible de justifier : le statut actuel est '{absence.Status}'. Seuls ABSENT et LATE sont autorisés.");

            return await _sessionsRepository.JustifyAbsenceAsync(sessionId, studentId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la justification de l'absence de l'étudiant {StudentId} dans la séance {SessionId}", studentId, sessionId);
            throw;
        }
    }

    /// <summary>
    /// Met à jour le statut d'une absence d'un étudiant dans une séance
    /// Seuls les statuts ABSENT et LATE sont autorisés
    /// </summary>
    public async Task<bool> UpdateAbsenceStatusAsync(string sessionId, string studentId, UpdateAbsenceStatusRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Mise à jour du statut d'absence de l'étudiant {StudentId} dans la séance {SessionId}", studentId, sessionId);

            if (request.Status != StatusPresence.ABSENT && request.Status != StatusPresence.LATE)
                throw new InvalidOperationException($"Le statut '{request.Status}' n'est pas autorisé. Seuls ABSENT et LATE sont acceptés.");

            var session = await _sessionsRepository.GetSessionsByIdAsync(sessionId)
                ?? throw new KeyNotFoundException($"La séance '{sessionId}' est introuvable.");

            var absence = session.Absences.FirstOrDefault(a => a.StudentId == studentId)
                ?? throw new KeyNotFoundException($"Aucune absence trouvée pour l'étudiant '{studentId}' dans la séance '{sessionId}'.");

            if (absence.Status == request.Status)
                throw new InvalidOperationException($"L'absence est déjà au statut '{request.Status}'.");

            return await _sessionsRepository.UpdateAbsenceStatusAsync(sessionId, studentId, request.Status, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour du statut d'absence de l'étudiant {StudentId} dans la séance {SessionId}", studentId, sessionId);
            throw;
        }
    }

    /// <summary>
    /// Met à jour le statut de plusieurs absences en une seule opération
    /// Seuls les statuts ABSENT et LATE sont autorisés
    /// </summary>
    public async Task<bool> UpdateAbsencesBulkAsync(string sessionId, List<UpdateAbsencesBulkItem> items, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Mise à jour en masse des absences de la séance {SessionId}", sessionId);

            if (items == null || !items.Any())
                throw new ArgumentException("La liste des absences est vide.");

            var session = await _sessionsRepository.GetSessionsByIdAsync(sessionId)
                ?? throw new KeyNotFoundException($"La séance '{sessionId}' est introuvable.");

            foreach (var item in items)
            {
                if (item.Status != StatusPresence.ABSENT && item.Status != StatusPresence.LATE)
                    throw new InvalidOperationException($"Le statut '{item.Status}' n'est pas autorisé. Seuls ABSENT et LATE sont acceptés.");

                var absence = session.Absences.FirstOrDefault(a => a.StudentId == item.StudentId)
                    ?? throw new KeyNotFoundException($"Aucune absence trouvée pour l'étudiant '{item.StudentId}' dans la séance '{sessionId}'.");
            }

            return await _sessionsRepository.UpdateAbsencesBulkAsync(sessionId, items, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour en masse des absences de la séance {SessionId}", sessionId);
            throw;
        }
    }

    /// <summary>
    /// Valide une séance et marque les absences des étudiants
    /// Seul le professeur de la séance peut la valider
    /// </summary>
    public async Task ValidateAndMarkAsync(string sessionId, MarkAbsencesRequest request)
{
    var session = await _sessionsRepository.GetSessionsByIdAsync(sessionId)
        ?? throw new KeyNotFoundException($"La séance '{sessionId}' est introuvable.");

    if (session.IsValidated)
        throw new InvalidOperationException("La séance est déjà validée.");

    foreach (var mark in request.Marks)
    {
        var absence = session.Absences.FirstOrDefault(a => a.Id == mark.AbsenceId)
            ?? throw new KeyNotFoundException($"Absence introuvable dans cette séance : {mark.AbsenceId}");

        absence.Status = mark.Status;
    
    }

    session.IsValidated = true;

    var ok = await _sessionsRepository.UpdateSessionsAsync(sessionId, session);
    if (!ok) throw new InvalidOperationException("Échec de la mise à jour de la séance.");

    _logger.LogInformation("Séance {SessionId} validée avec succès", sessionId);
}    

    /// <summary>
    /// Récupère les présences d'un étudiant avec les détails de chaque séance
    /// </summary>
    public async Task<List<StudentAttendanceDto>> GetMyAttendanceAsync(string studentId, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Récupération des présences de l'étudiant {StudentId}", studentId);

            var sessions = await _sessionsRepository.GetSessionsWithStudentAbsenceAsync(studentId, ct);

            return sessions
                .Select(s =>
                {
                    var myAbs = s.Absences.First(a => a.StudentId == studentId);

                    return new StudentAttendanceDto(
                        SessionId: s.Id,
                        StartTime: s.StartTime,
                        EndTime: s.EndTime,
                        Group: s.Group,
                        TeacherId: s.TeacherId,
                        Status: myAbs.Status,
                        JustificationDate: myAbs.JustificationDate
                    );
                })
                .OrderByDescending(x => x.StartTime)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des présences de l'étudiant {StudentId}", studentId);
            throw;
        }
    }
}