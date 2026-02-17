namespace StudentAttendance.src.StudentAttendance.Application.Interfaces.Repositories;

/// <summary>
/// Contrat générique pour les opérations CRUD communes à toutes les entités
/// </summary>
/// <typeparam name="T">Type de l'entité</typeparam>
public interface IBaseRepository<T> where T : class
{
    /// <summary>
    /// Récupère une entité par son identifiant
    /// </summary>
    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère toutes les entités de la collection
    /// </summary>
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Insère une seule entité
    /// </summary>
    Task InsertOneAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Insère une liste d'entités en une seule opération
    /// </summary>
    Task InsertManyAsync(List<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour une entité existante
    /// </summary>
    Task UpdateAsync(string id, T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime une entité par son identifiant
    /// </summary>
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}