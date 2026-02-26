using StudentAttendance.src.StudentAttendance.Application.DTOs.Absence;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Requests;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Response;
using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;

/// <summary>
/// Contrat des opérations métier liées aux séances
/// </summary>
public interface ISessionsService
{
    /// <summary>
    /// Récupère toutes les séances
    /// </summary>
    Task<List<SessionResponse>> GetAllSessionsAsync();

    /// <summary>
    /// Récupère une séance par son identifiant
    /// </summary>
    Task<SessionResponse?> GetSessionsByIdAsync(string id);

    /// <summary>
    /// Récupère les séances d'un professeur
    /// </summary>
    Task<List<SessionResponse>> GetSessionsByTeacherIdAsync(string teacherId);

    /// <summary>
    /// Récupère les séances d'un groupe
    /// </summary>
    Task<List<SessionResponse>> GetSessionsByGroupName(string group);

    /// <summary>
    /// Crée une nouvelle séance avec les absences des étudiants
    /// </summary>
    Task<SessionResponse> CreateSessionsAsync(CreateSessionRequest sessionrequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour une séance existante
    /// </summary>
Task<SessionResponse?> UpdateSessionsAsync(string id, UpdateSessionRequest sessionrequest);
    /// <summary>
    /// Supprime une séance
    /// </summary>
    Task<bool> DeleteSessionsAsync(string id);

    /// <summary>
    /// Récupère les étudiants d'une séance
    /// </summary>
    Task<List<User>> GetStudentsBySessionIdAsync(string sessionId);

    /// <summary>
    /// Récupère le professeur d'une séance
    /// </summary>
    Task<string?> GetProfessurBySessionIdAsync(string sessionId);

    /// <summary>
    /// Justifie une absence d'un étudiant dans une séance
    /// La séance doit être validée et le statut doit être ABSENT ou LATE
    /// </summary>
    Task<bool> JustifyAbsenceAsync(string sessionId, string studentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour le statut d'une absence d'un étudiant dans une séance
    /// </summary>
    Task<bool> UpdateAbsenceStatusAsync(string sessionId, string studentId, UpdateAbsenceStatusRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour le statut de plusieurs absences en une seule opération
    /// </summary>
    Task<bool> UpdateAbsencesBulkAsync(string sessionId, List<UpdateAbsencesBulkItem> items, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valide une séance et marque les absences des étudiants
    /// </summary>
Task ValidateAndMarkAsync(string sessionId, MarkAbsencesRequest request);

    /// <summary>
    /// Récupère les présences d'un étudiant avec les détails de chaque séance
    /// </summary>
    Task<List<StudentAttendanceDto>> GetMyAttendanceAsync(string studentId, CancellationToken ct = default);
}