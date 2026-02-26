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

        /// <summary>
        /// Récupère toutes les séances
        /// </summary>
        public async Task<List<Session>> GetAllSessionsAsync()
        {
            var docs = await _sessionsCollection.Find(_ => true).ToListAsync().ConfigureAwait(false);
            return docs.Select(SessionMapper.ToDomain).ToList();
        }

        /// <summary>
        /// Récupère une séance par son identifiant avec token d'annulation
        /// </summary>
        public async Task<Session?> GetByIdAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            var doc = await _sessionsCollection
                .Find(s => s.Id == sessionId)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return doc is null ? null : SessionMapper.ToDomain(doc);
        }

        /// <summary>
        /// Récupère une séance par son identifiant
        /// </summary>
        public Task<Session?> GetSessionsByIdAsync(string id)
            => GetByIdAsync(id, CancellationToken.None);

        /// <summary>
        /// Récupère les étudiants d'une séance via le groupe
        /// </summary>
        public async Task<List<User>> GetStudentsBySessionIdAsync(string sessionId)
        {
            var session = await _sessionsCollection
                .Find(s => s.Id == sessionId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (session is null) return new List<User>();

            var group = await _groupsCollection
                .Find(g => g.Label == session.Group)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (group is null) return new List<User>();

            var studentsDocs = await _usersCollection
                .Find(u => u.GroupId == group.Id && u.Role == Role.STUDENT && u.IsActive)
                .ToListAsync()
                .ConfigureAwait(false);

            return studentsDocs.Select(UserMapper.ToDomain).ToList();
        }

        /// <summary>
        /// Récupère le professeur d'une séance
        /// </summary>
        public async Task<string?> GetProfessurBySessionIdAsync(string sessionId)
        {
            var session = await _sessionsCollection
                .Find(s => s.Id == sessionId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return session?.TeacherId;
        }

        /// <summary>
        /// Récupère les séances d'un groupe
        /// </summary>
        public async Task<List<Session>> GetSessionsByGroupName(string group)
        {
            var docs = await _sessionsCollection
                .Find(s => s.Group == group)
                .ToListAsync()
                .ConfigureAwait(false);

            return docs.Select(SessionMapper.ToDomain).ToList();
        }

        /// <summary>
        /// Récupère les séances d'un professeur
        /// </summary>
        public async Task<List<Session>> GetSessionsByTeacherIdAsync(string teacherId)
        {
            var docs = await _sessionsCollection
                .Find(s => s.TeacherId == teacherId)
                .ToListAsync()
                .ConfigureAwait(false);

            return docs.Select(SessionMapper.ToDomain).ToList();
        }

        /// <summary>
        /// Crée une nouvelle séance
        /// </summary>
        public async Task<Session> CreateSessionsAsync(Session session)
        {
            var doc = SessionMapper.ToDocument(session);

            await _sessionsCollection
                .InsertOneAsync(doc)
                .ConfigureAwait(false);

            return SessionMapper.ToDomain(doc);
        }

        /// <summary>
        /// Met à jour une séance existante
        /// </summary>
        public async Task<bool> UpdateSessionsAsync(string id, Session session)
        {
            var doc = SessionMapper.ToDocument(session);

            var result = await _sessionsCollection
                .ReplaceOneAsync(s => s.Id == id, doc)
                .ConfigureAwait(false);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        /// <summary>
        /// Supprime une séance
        /// </summary>
        public async Task<bool> DeleteSessionsAsync(string id)
        {
            var result = await _sessionsCollection
                .DeleteOneAsync(s => s.Id == id)
                .ConfigureAwait(false);

            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        /// <summary>
        /// Vérifie si une séance existe
        /// </summary>
        public async Task<bool> ExistsSessionAsync(string id)
        {
            var count = await _sessionsCollection
                .CountDocumentsAsync(s => s.Id == id)
                .ConfigureAwait(false);

            return count > 0;
        }

        /// <summary>
        /// Valide une séance en passant IsValidated à true
        /// </summary>
        public async Task ValidateAsync(string sessionID, CancellationToken cancellationToken = default)
        {
            var filter = Builders<SessionDocument>.Filter.Eq(s => s.Id, sessionID);
            var update = Builders<SessionDocument>.Update.Set(s => s.IsValidated, true);

            await _sessionsCollection
                .UpdateOneAsync(filter, update, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Récupère les séances contenant une absence pour un étudiant donné
        /// </summary>
        public async Task<List<Session>> GetSessionsWithStudentAbsenceAsync(string studentId, CancellationToken ct = default)
        {
            var filter = Builders<SessionDocument>.Filter.ElemMatch(
                s => s.Absences,
                a => a.StudentId == studentId
            );

            var docs = await _sessionsCollection.Find(filter).ToListAsync(ct);
            return docs.Select(SessionMapper.ToDomain).ToList();
        }

        /// <summary>
        /// Met à jour le statut d'une absence à JUSTIFIED dans la session embedded
        /// </summary>
        public async Task<bool> JustifyAbsenceAsync(string sessionId, string studentId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<SessionDocument>.Filter.And(
                Builders<SessionDocument>.Filter.Eq(s => s.Id, sessionId),
                Builders<SessionDocument>.Filter.ElemMatch(s => s.Absences, a => a.StudentId == studentId)
            );

            var update = Builders<SessionDocument>.Update
                .Set("absences.$.status", (int)StatusPresence.JUSTIFIED)
                .Set("absences.$.justificationDate", DateTime.UtcNow);

            var result = await _sessionsCollection
                .UpdateOneAsync(filter, update, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        /// <summary>
        /// Met à jour le statut d'une absence dans la session embedded
        /// </summary>
        public async Task<bool> UpdateAbsenceStatusAsync(string sessionId, string studentId, StatusPresence status, CancellationToken cancellationToken = default)
        {
            var filter = Builders<SessionDocument>.Filter.And(
                Builders<SessionDocument>.Filter.Eq(s => s.Id, sessionId),
                Builders<SessionDocument>.Filter.ElemMatch(s => s.Absences, a => a.StudentId == studentId)
            );

            var update = Builders<SessionDocument>.Update
                .Set("absences.$.status", (int)status)
                .Set("absences.$.justificationDate", (DateTime?)null);

            var result = await _sessionsCollection
                .UpdateOneAsync(filter, update, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        /// <summary>
        /// Met à jour le statut de plusieurs absences en une seule opération
        /// </summary>
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
                    .Set("absences.$.status", (int)item.Status)
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