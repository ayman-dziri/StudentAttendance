using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Domain.Repositories
{
    public interface IGroupRepository
    {
        Task<List<Group>> GetAllGroupsAsync();
        Task<Group?> GetGroupByIdAsync(string id);
        Task<Group?> GetGroupByLabelAsync(string label);
        Task<Group> CreateGroupAsync(Group group);
        
        /*Task<bool> UpdateGroupAsync(string id, Group group);
        Task<bool> DeleteGroupAsync(string id);
        Task<bool> ExistsGroupAsync(string id);*/
    }
}