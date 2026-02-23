using Microsoft.Extensions.Logging;
using StudentAttendance.src.StudentAttendance.Application.Exceptions;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Enums;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Absence;

namespace StudentAttendance.src.StudentAttendance.Application.Services;

/// <summary>
/// Opérations métier liées aux absences
/// </summary>
public class AbsenceService : IAbsenceService
{
    private readonly IAbsenceRepository _absenceRepository;
    private readonly ILogger<AbsenceService> _logger;
    private readonly ISessionsRepository _sessions;

    public AbsenceService(
        IAbsenceRepository absenceRepository,
        ILogger<AbsenceService> logger,
        ISessionsRepository sessions)
    {
        _absenceRepository = absenceRepository;
        _logger = logger;
        _sessions = sessions;
    }

    /// <inheritdoc />
    public async Task CreateAbsencesForSessionAsync(string sessionId, List<string> studentIds, CancellationToken cancellationToken = default)
    {
        if (studentIds.Count == 0) return;

        var absences = studentIds.Select(studentId => new Absence
        {
            StudentId = studentId,
            SessionId = sessionId,
            Status = StatusPresence.PRESENT,
            JustificationDate = null
        }).ToList();

        await _absenceRepository.InsertManyAsync(absences, cancellationToken);

        _logger.LogInformation("{Count} absences créées pour la séance {SessionId}", absences.Count, sessionId);
    }

    /// <inheritdoc />
    public async Task<List<Absence>> GetAbsencesBySessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        return await _absenceRepository.GetBySessionIdAsync(sessionId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task JustifyAbsenceAsync(string absenceId, CancellationToken cancellationToken = default)
    {
        var absence = await _absenceRepository.GetByIdAsync(absenceId, cancellationToken)
            ?? throw new AbsenceNotFoundException(absenceId);

        if (absence.Status == StatusPresence.JUSTIFIED)
            throw new AbsenceAlreadyJustifiedException(absenceId);

        if (absence.Status == StatusPresence.PRESENT)
            throw new InvalidOperationException("Impossible de justifier une absence avec le statut PRESENT");

        absence.Status = StatusPresence.JUSTIFIED;
        absence.JustificationDate = DateTime.UtcNow;

        await _absenceRepository.UpdateAsync(absence, cancellationToken);

        _logger.LogInformation("Absence {AbsenceId} justifiée avec succès", absenceId);
    }

    //method update absence status to ABSENT (hna tzidha a ibrahim melhaoui)

    public async Task UpdateAbsencesBulkAsync(List<UpdateAbsenceStatusRequest> updates, CancellationToken cancellationToken = default)
    {
        if (updates.Count == 0) return;

        foreach (var u in updates)
        {
            var absence = await _absenceRepository.GetByIdAsync(u.AbsenceId, cancellationToken)
                ?? throw new AbsenceNotFoundException(u.AbsenceId);

            if (absence.Status == StatusPresence.JUSTIFIED)
                throw new InvalidOperationException("Absence already JUSTIFIED (admin-only).");

            if (!Enum.TryParse<StatusPresence>(u.Status, ignoreCase: true, out var newStatus))
                throw new InvalidOperationException("Invalid status. Allowed: PRESENT, ABSENT, LATE");

            if (newStatus == StatusPresence.JUSTIFIED)
                throw new InvalidOperationException("Do not set JUSTIFIED here.");

            absence.Status = newStatus;
            await _absenceRepository.UpdateAsync(absence, cancellationToken);
        }
    }

    private static string MapStatus(StatusPresence status) => status switch
    {
        StatusPresence.PRESENT => "PRESENT",
        StatusPresence.ABSENT => "ABSENT",
        StatusPresence.LATE => "LATE",
        StatusPresence.JUSTIFIED => throw new InvalidOperationException("JUSTIFIED is admin-only."),
        _ => throw new ArgumentOutOfRangeException(nameof(status))
    };

    public async Task ValidateAndMarkAsync(string teacherId, string sessionId, MarkAbsencesRequest request)
    {
        var session = await _sessions.GetSessionsByIdAsync(sessionId)
            ?? throw new Exception("Session not found");

        if (session.TeacherId != teacherId)
            throw new Exception("Unauthorized");

        if (session.IsValidated)
            throw new Exception("Session already validated");

        // Build bulk updates
        var updates = new List<UpdateAbsenceStatusRequest>();

        foreach (var mark in request.Marks)
        {
            var absence = await _absenceRepository.GetByStudentAndSessionAsync(mark.StudentId, sessionId);

            if (absence is null)
                throw new Exception($"Absence not found for student {mark.StudentId} in session {sessionId}. Make sure absences are created on session creation.");

            updates.Add(new UpdateAbsenceStatusRequest
            {
                AbsenceId = absence.Id,
                Status = MapStatus(mark.Status)
            });
        }

        await UpdateAbsencesBulkAsync(updates);

        // Validate session
        await _sessions.ValidateAsync(sessionId);
    }

    public async Task<List<AbsenceDto>> GetMyAbsencesAsync(string studentId)
    {
        // Ton implémentation existante ici (ou simple):
        var absences = await _absenceRepository.GetByStudentIdAsync(studentId);
        return absences
            .Where(a => a.Status != StatusPresence.PRESENT) // optionnel si tu veux que l'étudiant voit que non-présents
            .Select(a => new AbsenceDto(a.Id, a.SessionId, a.Status, a.JustificationDate))
            .ToList();
    }
}
