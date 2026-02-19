using Microsoft.Extensions.Logging;
using StudentAttendance.src.StudentAttendance.Application.Exceptions;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;

namespace StudentAttendance.src.StudentAttendance.Application.Services;

/// <summary>
/// Validation des conflits horaires des séances
/// </summary>
public class SessionConflictValidator : ISessionConflictValidator
{
    private readonly ISessionsRepository _sessionRepository;
    private readonly ILogger<SessionConflictValidator> _logger;

    public SessionConflictValidator(ISessionsRepository sessionRepository, ILogger<SessionConflictValidator> logger)
    {
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task ValidateNoConflictAsync(Session session, string? excludeSessionId = null, CancellationToken cancellationToken = default)
    {
        await ValidateGroupConflictAsync(session, excludeSessionId);
        await ValidateTeacherConflictAsync(session, excludeSessionId);
    }

    /// <summary>
    /// Vérifie qu'un groupe n'a pas deux séances au même moment
    /// </summary>
    private async Task ValidateGroupConflictAsync(Session session, string? excludeSessionId)
    {
        var groupSessions = await _sessionRepository.GetSessionsByGroupName(session.Group);

        var conflict = groupSessions.FirstOrDefault(s =>
            s.Id != excludeSessionId &&
            HasTimeOverlap(session.StartTime, session.EndTime, s.StartTime, s.EndTime));

        if (conflict is not null)
        {
            _logger.LogWarning("Conflit horaire détecté pour le groupe {Group}: séance {ConflictId}", session.Group, conflict.Id);
            throw new ConflictScheduleException(
                $"Conflit horaire : le groupe '{session.Group}' a déjà une séance de {conflict.StartTime:HH:mm} à {conflict.EndTime:HH:mm}");
        }
    }

    /// <summary>
    /// Vérifie qu'un professeur n'a pas deux séances au même moment
    /// </summary>
    private async Task ValidateTeacherConflictAsync(Session session, string? excludeSessionId)
    {
        var teacherSessions = await _sessionRepository.GetSessionsByTeacherIdAsync(session.TeacherId);

        var conflict = teacherSessions.FirstOrDefault(s =>
            s.Id != excludeSessionId &&
            HasTimeOverlap(session.StartTime, session.EndTime, s.StartTime, s.EndTime));

        if (conflict is not null)
        {
            _logger.LogWarning("Conflit horaire détecté pour le professeur {TeacherId}: séance {ConflictId}", session.TeacherId, conflict.Id);
            throw new ConflictScheduleException(
                $"Conflit horaire : le professeur a déjà une séance de {conflict.StartTime:HH:mm} à {conflict.EndTime:HH:mm}");
        }
    }

    /// <summary>
    /// Vérifie si deux plages horaires se chevauchent
    /// </summary>
    private static bool HasTimeOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
    {
        return start1 < end2 && start2 < end1;
    }
}