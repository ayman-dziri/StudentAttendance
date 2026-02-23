using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Domain.Repositories
{
    public interface IGroupRepository
    {
        Task<List<Group>> GetAllGroupsAsync(CancellationToken cancellationToken = default);
        Task<Group?> GetGroupByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<Group?> GetGroupByLabelAsync(string label, CancellationToken cancellationToken = default);
        Task<Group> CreateGroupAsync(Group group, CancellationToken cancellationToken = default);
        Task<bool> UpdateGroupAsync(string id, Group group, CancellationToken cancellationToken = default);
        Task<bool> DeleteGroupAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByLabelAsync(string label, CancellationToken cancellationToken = default);
    }
}