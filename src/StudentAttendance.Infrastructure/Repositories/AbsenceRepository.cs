using MongoDB.Driver;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Infrastructure.Collections;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
using StudentAttendance.src.StudentAttendance.Infrastructure.Documents;
using StudentAttendance.src.StudentAttendance.Infrastructure.Mappers;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Repositories;

/// <summary>
/// Accès aux données MongoDB pour les absences
/// </summary>
public class AbsenceRepository : IAbsenceRepository
{
    private readonly IMongoCollection<AbsenceDocument> _collection;

    public AbsenceRepository(StudentAttendanceDbContext context)
    {
        _collection = context.GetCollection<AbsenceDocument>(CollectionNames.Absences);
    }

    /// <inheritdoc />
    public async Task InsertManyAsync(List<Absence> absences, CancellationToken cancellationToken = default)
    {
        if (absences.Count == 0) return;

        var documents = absences.Select(AbsenceMapper.ToDocument).ToList();
        await _collection.InsertManyAsync(documents, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Absence>> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<AbsenceDocument>.Filter.Eq(d => d.SessionId, sessionId);
        var documents = await _collection.Find(filter).ToListAsync(cancellationToken);
        return documents.Select(AbsenceMapper.ToDomain).ToList();
    }

    /// <inheritdoc />
    public async Task<Absence?> GetByIdAsync(string absenceId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<AbsenceDocument>.Filter.Eq(d => d.Id, absenceId);
        var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return document is null ? null : AbsenceMapper.ToDomain(document);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Absence absence, CancellationToken cancellationToken = default)
    {
        var document = AbsenceMapper.ToDocument(absence);
        var filter = Builders<AbsenceDocument>.Filter.Eq(d => d.Id, document.Id);
        await _collection.ReplaceOneAsync(filter, document, cancellationToken: cancellationToken);
    }

    Task<List<Absence>> IAbsenceRepository.GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<Absence>> UpdateAsync(string id, Absence absence, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}