using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;

using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
using StudentAttendance.src.StudentAttendance.Infrastructure.Documents;
using StudentAttendance.src.StudentAttendance.Infrastructure.Interfaces;
using StudentAttendance.src.StudentAttendance.Infrastructure.Mappers;
using System.Text.RegularExpressions;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Repositories;

/// <summary>
/// Accès aux données MongoDB pour les absences
/// </summary>
public class AbsenceRepository : IAbsenceRepository
{
    private readonly IMongoCollection<AbsenceDocument> _collection;

    public AbsenceRepository(IMongoClientFactory mongoClientFactory, IOptions<MongoDbSettings> options)
    {
        var collectionAbsences = options.Value.Collections?["Absences"] ?? "Absences";


        _collection = mongoClientFactory.GetMongoCollection<AbsenceDocument>(collectionAbsences);
    }


    

    /// <inheritdoc />
    public async Task InsertManyAsync(List<Absence> absences, CancellationToken cancellationToken = default)
    {
        if (absences.Count == 0) return;

        var documents = absences.Select(AbsenceMapper.ToDocument).ToList();
        await _collection.InsertManyAsync(documents, cancellationToken: cancellationToken);
    }

    

    public async Task InsertOneAsync(Absence absence, CancellationToken cancellationToken = default)
    {
        var document = AbsenceMapper.ToDocument(absence);
        await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
    }

    public async Task<List<Absence>> GetAllAbsences(CancellationToken cancellationToken = default)
    {
        var documents = await _collection.Find(_ => true).ToListAsync(cancellationToken);

        return documents.Select(AbsenceMapper.ToDomain).ToList();
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

    public async Task<Absence?> GetByStudentAndSessionAsync(string studentId, string sessionId)
    {
        var filter = Builders<AbsenceDocument>.Filter.And(
            Builders<AbsenceDocument>.Filter.Eq(x => x.StudentId, studentId),
            Builders<AbsenceDocument>.Filter.Eq(x => x.SessionId, sessionId)
        );

        var document = await _collection.Find(filter).FirstOrDefaultAsync();
        return document is null ? null : AbsenceMapper.ToDomain(document);
    }

    public async Task<List<Absence>> GetByStudentIdAsync(string studentId)
    {
        var filter = Builders<AbsenceDocument>.Filter.Eq(x => x.StudentId, studentId);

        var documents = await _collection.Find(filter).ToListAsync();
        return documents.Select(AbsenceMapper.ToDomain).ToList();
    }
}