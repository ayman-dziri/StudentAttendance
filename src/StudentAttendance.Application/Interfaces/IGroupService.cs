using StudentAttendance.src.StudentAttendance.Application.DTOs.Group;

namespace StudentAttendance.src.StudentAttendance.Application.Interfaces
{
    public interface IGroupService
    {
        Task<List<GroupResponseDto>> GetAllGroupsAsync();
        Task<GroupResponseDto?> GetGroupByIdAsync(string id);
        Task<GroupResponseDto?> GetGroupByLabelAsync(string label);
        Task<GroupResponseDto> CreateGroupAsync(CreateGroupDto groupDto);

        /*Task<bool> UpdateGroupAsync(string id, UpdateGroupDto groupDto);
        Task<bool> DeleteGroupAsync(string id);
        Task<bool> ExistsGroupAsync(string id);*/
    }
}
