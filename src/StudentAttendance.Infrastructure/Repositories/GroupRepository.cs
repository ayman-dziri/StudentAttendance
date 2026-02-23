using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
using StudentAttendance.src.StudentAttendance.Infrastructure.Documents;
using StudentAttendance.src.StudentAttendance.Infrastructure.Interfaces;
using StudentAttendance.src.StudentAttendance.Infrastructure.Mappers;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IMongoCollection<GroupDocument> _groupCollection;

        public GroupRepository(
            IMongoClientFactory mongoClientFactory,
            IOptions<MongoDbSettings> options)
        {
            var collectionGroups = options.Value.Collections?["Groups"] ?? "Groups";

            _groupCollection = mongoClientFactory
                .GetMongoCollection<GroupDocument>(collectionGroups);
        }

        public async Task<Group?> GetByNameAsync(string label,CancellationToken ct = default)
        {
            var doc = await _groupCollection
                .Find(g => g.Label == label)
                .FirstOrDefaultAsync(ct);

            return doc is null
                ? null
                : GroupMapper.ToDomain(doc);
        }

        public async Task<Group> CreateAsync(Group group, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(group.Id))
                group.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

            var doc = GroupMapper.ToDocument(group);

            await _groupCollection
                .InsertOneAsync(doc, cancellationToken: ct)
                .ConfigureAwait(false);

            return GroupMapper.ToDomain(doc);
        }
    }
}