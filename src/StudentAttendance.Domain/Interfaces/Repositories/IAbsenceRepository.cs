using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;

/// <summary>
/// Contrat d'accès aux données pour les absences
/// </summary>
public interface IAbsenceRepository
{

    Task<List<Absence>> GetAllAbsences(CancellationToken cancellationToken = default);

    Task InsertOneAsync(Absence absence , CancellationToken cancellationToken = default);
    /// <summary>
    /// Insère une liste d'absences en une seule opération
    /// </summary>
    Task InsertManyAsync(List<Absence> absences, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère toutes les absences liées à une séance
    /// </summary>
    Task<List<Absence>> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère une absence par son identifiant
    /// </summary>
    Task<Absence?> GetByIdAsync(string absenceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour une absence existante
    /// </summary>
    Task UpdateAsync(Absence absence, CancellationToken cancellationToken = default);
}