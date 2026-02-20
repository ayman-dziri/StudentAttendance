using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StudentAttendance.src.StudentAttendance.Application.Intefaces;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
using StudentAttendance.src.StudentAttendance.Infrastructure.Interfaces;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Repositories
{
    public class SessionsRepository : ISessionsRepository
    {
        private readonly IMongoCollection<Session> _sessionsCollection;

        public SessionsRepository(IMongoClientFactory mongoClientFactory, IOptions<MongoDbSettings> options)
        {
            var collectionName = options.Value.Collections?["Sessions"] ?? "Sessions";
            _sessionsCollection = mongoClientFactory.GetMongoCollection<Session>("Sessions");
        }


        public async Task<List<Session>> GetAllSessionsAsync() {
            return await _sessionsCollection.Find(_ => true).ToListAsync().ConfigureAwait(false);
        }

        public async Task<Session?> GetSessionsByIdAsync(string id) {

            return await _sessionsCollection.Find(session => session.Id == id).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<List<Session>> GetSessionsByGroupName(string group)
        {

            return await _sessionsCollection.Find(session => session.Group == group).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<Session>> GetSessionsByTeacherIdAsync(string teacherId) {
            return await _sessionsCollection.Find(session => session.TeacherId == teacherId).ToListAsync().ConfigureAwait(false);
        }

        public async Task<Session> CreateSessionsAsync(Session session)
        {
            await _sessionsCollection.InsertOneAsync(session)
                .ConfigureAwait(false);
            return session;
        }

        public async Task<bool> UpdateSessionsAsync(string id, Session session)
        {
            var result = await _sessionsCollection.ReplaceOneAsync(s => s.Id == id, session)
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
