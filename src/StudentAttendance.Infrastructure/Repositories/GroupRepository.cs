using MongoDB.Driver;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Repositories;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
using StudentAttendance.src.StudentAttendance.Infrastructure.Documents;
using StudentAttendance.src.StudentAttendance.Infrastructure.Mappers;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IMongoCollection<GroupDocument> _groupsCollection;

        public GroupRepository(StudentAttendanceDbContext context)
        {
            _groupsCollection = context.GetCollection<GroupDocument>("Groups");
        }
        public async Task<List<Group>> GetAllGroupsAsync()
        {
            var groupDocuments = await _groupsCollection.Find(_ => true).ToListAsync().ConfigureAwait(false);
            return groupDocuments.Select(GroupMapper.ToDomain).ToList();
        }
        public async Task<Group?> GetGroupByIdAsync(string id)
        {
            var groupDocument = await _groupsCollection.Find(g => g.Id == id).FirstOrDefaultAsync().ConfigureAwait(false);
            if (groupDocument is null) return null;
            return GroupMapper.ToDomain(groupDocument);
        }
        public async Task<Group?> GetGroupByLabelAsync(string label)
        {
            var groupDocument = await _groupsCollection.Find(g => g.Label == label).FirstOrDefaultAsync().ConfigureAwait(false);
            if (groupDocument is null) return null;
            return GroupMapper.ToDomain(groupDocument);
        }
        public async Task<Group> CreateGroupAsync(Group group)
        {
            var groupDocument = GroupMapper.ToDocument(group);
            await _groupsCollection.InsertOneAsync(groupDocument).ConfigureAwait(false);
            return GroupMapper.ToDomain(groupDocument);
        }
    }
}