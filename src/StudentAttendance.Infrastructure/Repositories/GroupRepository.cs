using MongoDB.Bson;
using MongoDB.Driver;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Repositories;
using StudentAttendance.src.StudentAttendance.Infrastructure.Collections;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
using StudentAttendance.src.StudentAttendance.Infrastructure.Documents;
using StudentAttendance.src.StudentAttendance.Infrastructure.Mappers;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IMongoCollection<GroupDocument> _groupsCollection;

        public GroupRepository(MongoDbContext context)
        {
            _groupsCollection = context.GetCollection<GroupDocument>(CollectionNames.Groups);
        }
        public async Task<List<Group>> GetAllGroupsAsync(CancellationToken cancellationToken = default)
        {
            var groupDocuments = await _groupsCollection.Find(_ => true).ToListAsync(cancellationToken);
            return groupDocuments.Select(GroupMapper.ToDomain).ToList();
        }
        public async Task<Group?> GetGroupByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return null;
            var groupDocument = await _groupsCollection.Find(g => g.Id == id).FirstOrDefaultAsync(cancellationToken);
            if (groupDocument is null) return null;
            return GroupMapper.ToDomain(groupDocument);
        }
        public async Task<Group?> GetGroupByLabelAsync(string label, CancellationToken cancellationToken = default)
        {
            var groupDocument = await _groupsCollection.Find(g => g.Label == label).FirstOrDefaultAsync(cancellationToken);
            if (groupDocument is null) return null;
            return GroupMapper.ToDomain(groupDocument);
        }
        public async Task<Group> CreateGroupAsync(Group group, CancellationToken cancellationToken = default)
        {
            var groupDocument = GroupMapper.ToDocument(group);
            await _groupsCollection.InsertOneAsync(groupDocument, cancellationToken: cancellationToken);
            return GroupMapper.ToDomain(groupDocument);
        }
        public async Task<bool> UpdateGroupAsync(string id, Group group, CancellationToken cancellationToken = default)
        {
            var groupDocument = GroupMapper.ToDocument(group);
            var result = await _groupsCollection.ReplaceOneAsync(g => g.Id == id, groupDocument, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteGroupAsync(string id, CancellationToken cancellationToken = default)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return false;
            var result = await _groupsCollection.DeleteOneAsync(g => g.Id == id, cancellationToken: cancellationToken);
            return result.DeletedCount > 0;
        }
        public async Task<bool> ExistsByLabelAsync(string label, CancellationToken cancellationToken = default)
        {
            var count = await _groupsCollection.CountDocumentsAsync(g => g.Label == label, cancellationToken: cancellationToken);
            return count > 0;
        }
    }
}