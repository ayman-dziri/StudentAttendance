using StudentAttendance.src.StudentAttendance.Domain.Entities;  
namespace StudentAttendance.src.StudentAttendance.Application.Interfaces.Repositories;
///<summary>
/// Contrat d'accés aux données pour les absences
/// </summary>
public interface IAbsenceRepository
{
    ///<summary>
    /// Insere une liste d'absence en une seule operation
    /// </summary>
    Task InsertManyAsync (List<Absence> absences, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère toutes les absences liées à une séance
    /// </summary>
    Task<List<Absence>> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default);
    ///<summary>
    /// Récupèrer une absence papr sont identifiant
    /// </summary>
    Task<List<Absence>> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour une absence existante
    /// </summary>  
    Task<List<Absence>> UpdateAsync(string id, Absence absence, CancellationToken cancellationToken = default);
    
    }