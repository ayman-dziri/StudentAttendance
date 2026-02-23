using StudentAttendance.src.StudentAttendance.Application.DTOs.Group;

namespace StudentAttendance.src.StudentAttendance.Application.Interfaces
{
    public interface IGroupService
    {
        Task<List<GroupResponseDto>> GetAllGroupsAsync(CancellationToken cancellationToken = default);
        Task<GroupResponseDto> GetGroupByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<GroupResponseDto> GetGroupByLabelAsync(string label, CancellationToken cancellationToken = default);
        Task<GroupResponseDto> CreateGroupAsync(CreateGroupDto groupDto, CancellationToken cancellationToken = default);
        Task<bool> UpdateGroupAsync(string id, UpdateGroupDto groupDto, CancellationToken cancellationToken = default);
        Task<bool> DeleteGroupAsync(string id, CancellationToken cancellationToken = default);
    }
}
