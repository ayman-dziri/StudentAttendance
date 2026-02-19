using Microsoft.Extensions.Options;
using MongoDB.Driver;
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
            var docs = await _sessionsCollection.Find(_ => true).ToListAsync();
            return docs.Select(SessionMapper.ToDomain).ToList();
        }


        public async Task<Session?> GetSessionsByIdAsync(string id) {

            var doc = await _sessionsCollection.Find(session => session.Id == id).FirstOrDefaultAsync().ConfigureAwait(false);
            return doc == null ? null : SessionMapper.ToDomain(doc);
        }

        public async Task<Session?> GetByIdAsync(string sessionId , CancellationToken  cancellationToken = default )
        {

            var doc = await _sessionsCollection
            .Find(s => s.Id == sessionId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

            return doc == null ? null : SessionMapper.ToDomain(doc);
        }


        public async Task<List<User>> GetStudentsBySessionIdAsync(string sessionId)
        {
            var session = await _sessionsCollection.Find(session => session.Id == sessionId).FirstOrDefaultAsync().ConfigureAwait(false);
            
            if(session == null)
            {
                return new List<User>();
            }
            
            // Recuperer le groupe avec le label
            var group = await _groupsCollection.Find(group => group.Label == session.Group).FirstOrDefaultAsync().ConfigureAwait(false);


            if(group == null)
            {
                return new List<User>();
            }


            // Recuperer les étudiants de ce groupe
            var students = await _usersCollection.Find(user => user.GroupId == group.Id && user.Role == Role.STUDENT).ToListAsync().ConfigureAwait(false);



            return students.Select(UserMapper.ToDomain).ToList();
        }


        public async Task<User?> GetProfessurBySessionIdAsync(string sessionId)
        {
            //recuperer la session
            var session = await _sessionsCollection.Find(session => session.Id == sessionId).FirstOrDefaultAsync().ConfigureAwait(false);

            if (session == null)
            {
                throw new Exception($"Session with ID {sessionId} not found.");
            }

            // recuperer le groupe avec le label
            var group = await _groupsCollection.Find(group => group.Label == session.Group).FirstOrDefaultAsync().ConfigureAwait(false);


            if (group == null)
            {
                return null;
            }


            // Recuperer le professeur de ce groupe
            var professeurdoc = await _usersCollection.Find(user => user.GroupId == group.Id && user.Role == Role.TEACHER).FirstOrDefaultAsync().ConfigureAwait(false);

            if (professeurdoc == null)
            {
                return null;
            }

            return UserMapper.ToDomain(professeurdoc);
        }

        public async Task<List<Session>> GetSessionsByGroupName(string group)
        {

            var docs = await _sessionsCollection.Find(session => session.Group == group).ToListAsync().ConfigureAwait(false);
            return docs.Select(SessionMapper.ToDomain).ToList();
        }

        public async Task<List<Session>> GetSessionsByTeacherIdAsync(string teacherId) {
            var docs = await _sessionsCollection.Find(session => session.TeacherId == teacherId).ToListAsync().ConfigureAwait(false);
            return docs.Select(SessionMapper.ToDomain).ToList();
        }

        public async Task<Session> CreateSessionsAsync(Session session)
        {
            var doc = SessionMapper.ToDocument(session);
            await _sessionsCollection.InsertOneAsync(doc)
                .ConfigureAwait(false);
            return SessionMapper.ToDomain(doc);
        }

        public async Task<bool> UpdateSessionsAsync(string id, Session session)
        {
            var doc = SessionMapper.ToDocument(session);
            var result = await _sessionsCollection.ReplaceOneAsync(s => s.Id == id, doc)
                .ConfigureAwait(false);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteSessionsAsync(string id)
        {
            var result = await _sessionsCollection.DeleteOneAsync(s => s.Id == id)
                .ConfigureAwait(false);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }


        public async Task<bool> ExistsSessionAsync(string id)
        {
            var count = await _sessionsCollection.CountDocumentsAsync(s => s.Id == id)
                .ConfigureAwait(false);
            return count > 0;
        }


        public async Task ValidateAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<SessionDocument>.Filter.Eq(s => s.Id, sessionId);
            var update = Builders<SessionDocument>.Update.Set(s => s.IsValidated, true);

            await _sessionsCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }







    }
}
