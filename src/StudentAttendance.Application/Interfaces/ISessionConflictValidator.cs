using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;

/// <summary>
/// Contrat de validation des conflits horaires des séances
/// </summary>
public interface ISessionConflictValidator
{
    /// <summary>
    /// Vérifie qu'il n'y a pas de conflit horaire pour le groupe et le professeur.
    /// Lance ConflictScheduleException en cas de conflit.
    /// </summary>
    Task ValidateNoConflictAsync(Session session, string? excludeSessionId = null, CancellationToken cancellationToken = default);
}