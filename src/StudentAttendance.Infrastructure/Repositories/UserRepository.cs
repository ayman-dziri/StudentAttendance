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
    public class UserRepository : IUserRepository
    {

        private readonly IMongoCollection<UserDocument> _usersCollection;
        

        public UserRepository(IMongoClientFactory mongoClientFactory, IOptions<MongoDbSettings> options)
        {
            var collectionUsers = options.Value.Collections?["Users"] ?? "Users";


            _usersCollection = mongoClientFactory.GetMongoCollection<UserDocument>(collectionUsers);

        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var docs = await _usersCollection.Find(_ => true).ToListAsync().ConfigureAwait(false);
            return docs.Select(UserMapper.ToDomain).ToList();
        }

        public async Task<User> CreateUserAsync(User user)
        {
           
            if (string.IsNullOrWhiteSpace(user.Id))
                user.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

            var doc = UserMapper.ToDocument(user);

            await _usersCollection
                .InsertOneAsync(doc)
                .ConfigureAwait(false);

            return UserMapper.ToDomain(doc);
        }
        public async Task<List<User>> GetStudentsByGroupIdAsync(string groupId, CancellationToken ct = default)
        {
            var users = await _usersCollection
                .Find(u => u.GroupId == groupId
                           && u.Role == Role.STUDENT
                           && u.IsActive)
                .ToListAsync(ct);

            return users.Select(UserMapper.ToDomain).ToList();
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var userbyemail = await _usersCollection
                .Find(u => u.Email == email.Trim().ToLowerInvariant())
                .FirstOrDefaultAsync(cancellationToken);
            return userbyemail != null ? UserMapper.ToDomain(userbyemail) : null;
        }
    }
}
