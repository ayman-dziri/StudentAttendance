using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StudentAttendance.src.StudentAttendance.Application.Intefaces;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
using StudentAttendance.src.StudentAttendance.Infrastructure.Documents;
using StudentAttendance.src.StudentAttendance.Infrastructure.Interfaces;
using StudentAttendance.src.StudentAttendance.Infrastructure.Mappers;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Repositories
{
    public class SessionsRepository : ISessionsRepository
    {
        private readonly IMongoCollection<SessionDocument> _sessionsCollection;

        public SessionsRepository(IMongoClientFactory mongoClientFactory, IOptions<MongoDbSettings> options)
        {
            var collectionName = options.Value.Collections?["Sessions"] ?? "Sessions";
            _sessionsCollection = mongoClientFactory.GetMongoCollection<SessionDocument>(collectionName);
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
            return session;
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





    }
}
