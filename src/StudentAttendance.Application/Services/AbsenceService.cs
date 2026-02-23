using Microsoft.Extensions.Logging;
using StudentAttendance.src.StudentAttendance.Application.Exceptions;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Enums;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Application.DTOs.absence;

namespace StudentAttendance.src.StudentAttendance.Application.Services;

/// <summary>
/// Opérations métier liées aux absences
/// </summary>
public class AbsenceService : IAbsenceService
{
    private readonly IAbsenceRepository _absenceRepository;
    private readonly ILogger<AbsenceService> _logger;

    public AbsenceService(IAbsenceRepository absenceRepository, ILogger<AbsenceService> logger)
    {
        _absenceRepository = absenceRepository;
        _logger = logger;
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

    /// <inheritdoc />
 /// <inheritdoc />
    public async Task UpdateAbsenceStatusAsync(string absenceId, string status, CancellationToken cancellationToken = default)
    {
        var absence = await _absenceRepository.GetByIdAsync(absenceId, cancellationToken)
            ?? throw new AbsenceNotFoundException(absenceId);

        var parsedStatus = ParseStatus(status);

        if (absence.Status == StatusPresence.JUSTIFIED)
            throw new InvalidOperationException("Impossible de modifier une absence déjà justifiée");

        if (absence.Status == parsedStatus)
            throw new InvalidOperationException($"L'absence a déjà le statut {status}");

        absence.Status = parsedStatus;

        await _absenceRepository.UpdateAsync(absence, cancellationToken);

        _logger.LogInformation("Absence {AbsenceId} mise à jour : {Status}", absenceId, status);
    }

    /// <inheritdoc />
    public async Task UpdateAbsencesBulkAsync(List<UpdateAbsenceStatusRequest> updates, CancellationToken cancellationToken = default)
    {
        if (updates.Count == 0) return;

        foreach (var update in updates)
        {
            await UpdateAbsenceStatusAsync(update.AbsenceId, update.Status, cancellationToken);
        }

        _logger.LogInformation("{Count} absences mises à jour en masse", updates.Count);
    }

    /// <summary>
    /// Convertit un string en StatusPresence (PRESENT, ABSENT, LATE uniquement)
    /// </summary>
    private static StatusPresence ParseStatus(string status)
    {
        return status.ToUpper() switch
        {
            "PRESENT" => StatusPresence.PRESENT,
            "ABSENT" => StatusPresence.ABSENT,
            "LATE" => StatusPresence.LATE,
            _ => throw new InvalidOperationException($"Statut invalide : {status}. Valeurs autorisées : PRESENT, ABSENT, LATE")
        };
    }

    public Task UpdateMultipleAbsencesStatusAsync(List<string> absenceIds, string status, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}