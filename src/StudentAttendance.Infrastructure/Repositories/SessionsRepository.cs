using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Requests;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Enums;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
using StudentAttendance.src.StudentAttendance.Infrastructure.Documents;
using StudentAttendance.src.StudentAttendance.Infrastructure.Interfaces;
using StudentAttendance.src.StudentAttendance.Infrastructure.Mappers;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Repositories
{
    public class SessionsRepository : ISessionsRepository
    {
        private readonly IMongoCollection<SessionDocument> _sessionsCollection;
        private readonly IMongoCollection<GroupDocument> _groupsCollection;
        private readonly IMongoCollection<UserDocument> _usersCollection;

        public SessionsRepository(IMongoClientFactory mongoClientFactory, IOptions<MongoDbSettings> options)
        {
            var collectionSessions = options.Value.Collections?["Sessions"] ?? "Sessions";
            var collectionsGroups = options.Value.Collections?["Groups"] ?? "Groups";
            var collectionUsers = options.Value.Collections?["Users"] ?? "Users";

            _sessionsCollection = mongoClientFactory.GetMongoCollection<SessionDocument>(collectionSessions);
            _groupsCollection = mongoClientFactory.GetMongoCollection<GroupDocument>(collectionsGroups);
            _usersCollection = mongoClientFactory.GetMongoCollection<UserDocument>(collectionUsers);
        }

        public async Task<List<Session>> GetAllSessionsAsync()
        {
            var docs = await _sessionsCollection.Find(_ => true).ToListAsync().ConfigureAwait(false);
            return docs.Select(SessionMapper.ToDomain).ToList();
        }

        // Garde UNE seule méthode "Get by id" (avec token)
        public async Task<Session?> GetByIdAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            var doc = await _sessionsCollection
                .Find(s => s.Id == sessionId)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return doc is null ? null : SessionMapper.ToDomain(doc);
        }

        // Si ton interface exige GetSessionsByIdAsync, fais-la appeler GetByIdAsync
        public Task<Session?> GetSessionsByIdAsync(string id)
            => GetByIdAsync(id, CancellationToken.None);

        public async Task<List<User>> GetStudentsBySessionIdAsync(string sessionId)
        {
            // 1) Récupérer la session
            var session = await _sessionsCollection
                .Find(s => s.Id == sessionId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (session is null)
                return new List<User>();

            // 2) Récupérer le groupe via label (session.Group)
            var group = await _groupsCollection
                .Find(g => g.Label == session.Group)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (group is null)
                return new List<User>();

            // 3) Récupérer les étudiants via GroupId
            var studentsDocs = await _usersCollection
                .Find(u => u.GroupId == group.Id && u.Role == Role.STUDENT && u.IsActive)
                .ToListAsync()
                .ConfigureAwait(false);

            return studentsDocs.Select(UserMapper.ToDomain).ToList();
        }

        public async Task<string?> GetProfessurBySessionIdAsync(string sessionId)
        {
            var session = await _sessionsCollection
                .Find(s => s.Id == sessionId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            // Signature string? => retourne null si introuvable
            return session?.TeacherId;
        }

        public async Task<List<Session>> GetSessionsByGroupName(string group)
        {
            var docs = await _sessionsCollection
                .Find(s => s.Group == group)
                .ToListAsync()
                .ConfigureAwait(false);

            return docs.Select(SessionMapper.ToDomain).ToList();
        }

        public async Task<List<Session>> GetSessionsByTeacherIdAsync(string teacherId)
        {
            var docs = await _sessionsCollection
                .Find(s => s.TeacherId == teacherId)
                .ToListAsync()
                .ConfigureAwait(false);

            return docs.Select(SessionMapper.ToDomain).ToList();
        }

        public async Task<Session> CreateSessionsAsync(Session session)
        {
            var doc = SessionMapper.ToDocument(session);

            await _sessionsCollection
                .InsertOneAsync(doc)
                .ConfigureAwait(false);

            return SessionMapper.ToDomain(doc);
        }

        public async Task<bool> UpdateSessionsAsync(string id, Session session)
        {
            var doc = SessionMapper.ToDocument(session);

            var result = await _sessionsCollection
                .ReplaceOneAsync(s => s.Id == id, doc)
                .ConfigureAwait(false);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteSessionsAsync(string id)
        {
            var result = await _sessionsCollection
                .DeleteOneAsync(s => s.Id == id)
                .ConfigureAwait(false);

            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<bool> ExistsSessionAsync(string id)
        {
            var count = await _sessionsCollection
                .CountDocumentsAsync(s => s.Id == id)
                .ConfigureAwait(false);

            return count > 0;
        }

        public async Task ValidateAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<SessionDocument>.Filter.Eq(s => s.Id, sessionId);
            var update = Builders<SessionDocument>.Update.Set(s => s.IsValidated, true);

            await _sessionsCollection
                .UpdateOneAsync(filter, update, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        public async Task<bool> JustifyAbsenceAsync(string sessionId, string studentId,CancellationToken cancellationToken = default)
        {
            var filter = Builders<SessionDocument>.Filter.And(
                Builders<SessionDocument>.Filter.Eq(s => s.Id, sessionId),
                Builders<SessionDocument>.Filter.ElemMatch(s => s.Absences, a => a.StudentId == studentId)
            );
            var update =Builders<SessionDocument>.Update
                .Set("absences.$.status", StatusPresence.JUSTIFIED)
                .Set("Absences.$.JustificationDate", DateTime.UtcNow);
                
            var result = await _sessionsCollection
                .UpdateOneAsync(filter, update, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateAbsenceStatusAsync(string sessionId, string studentId, StatusPresence status, CancellationToken cancellationToken = default)
        {
            var filter = Builders<SessionDocument>.Filter.And(
                Builders<SessionDocument>.Filter.Eq(s => s.Id, sessionId),
                Builders<SessionDocument>.Filter.ElemMatch(s => s.Absences, a => a.StudentId == studentId)
            );
            var update = Builders<SessionDocument>.Update
                .Set("Absences.$.Status", status)
                .Set("Absences.$.JustificationDate", (DateTime?)null);


            var result = await _sessionsCollection
                .UpdateOneAsync(filter, update, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
        public async Task<bool> UpdateAbsencesBulkAsync(string sessionId, List<UpdateAbsencesBulkItem> items, CancellationToken cancellationToken = default)
{
    var updates = new List<WriteModel<SessionDocument>>();

    foreach (var item in items)
    {
        var filter = Builders<SessionDocument>.Filter.And(
            Builders<SessionDocument>.Filter.Eq(s => s.Id, sessionId),
            Builders<SessionDocument>.Filter.ElemMatch(s => s.Absences, a => a.StudentId == item.StudentId)
        );

        var update = Builders<SessionDocument>.Update
            .Set("absences.$.status", (Int32)item.Status)
            .Set("absences.$.justificationDate", (DateTime?)null);

        updates.Add(new UpdateOneModel<SessionDocument>(filter, update));
    }

    var result = await _sessionsCollection
        .BulkWriteAsync(updates, cancellationToken: cancellationToken)
        .ConfigureAwait(false);

    return result.IsAcknowledged && result.ModifiedCount > 0;
}
    }
}