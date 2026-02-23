using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Application.DTOs.absence;

namespace StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;

/// <summary>
/// Contrat des opérations métier liées aux absences
/// </summary>
public interface IAbsenceService
{
    /// <summary>
    /// Crée une absence avec le statut PRESENT pour chaque étudiant
    /// </summary>
    /// <param name="sessionId">Identifiant de la séance</param>
    /// <param name="studentIds">Liste des identifiants des étudiants</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task CreateAbsencesForSessionAsync(string sessionId, List<string> studentIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère la liste des absences d'une séance
    /// </summary>
    /// <param name="sessionId">Identifiant de la séance</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task<List<Absence>> GetAbsencesBySessionAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Justifie une absence en passant son statut à JUSTIFIED
    /// </summary>
    /// <param name="absenceId">Identifiant de l'absence</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task JustifyAbsenceAsync(string absenceId, CancellationToken cancellationToken = default);

    ///<summary>
    /// Met à jour le statut d'une absence (PRESENT, ABSENT, JUSTIFIED)
    ///</summary>
    Task UpdateAbsenceStatusAsync(string absenceId, string status, CancellationToken cancellationToken = default);

    /// <summary>
    /// mettre a jour le status de plusieur absence d' un coup
    /// </summary>
    Task UpdateAbsencesBulkAsync(List<UpdateAbsenceStatusRequest> updates, CancellationToken cancellationToken = default);
}