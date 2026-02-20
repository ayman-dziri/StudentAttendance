using StudentAttendance.src.StudentAttendance.Application.DTOs.Group;
using StudentAttendance.src.StudentAttendance.Application.Exceptions;
using StudentAttendance.src.StudentAttendance.Application.Interfaces;
using StudentAttendance.src.StudentAttendance.Application.Mappers;
using StudentAttendance.src.StudentAttendance.Domain.Repositories;

namespace StudentAttendance.src.StudentAttendance.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ILogger<GroupService> _logger;

        public GroupService(IGroupRepository groupRepository, ILogger<GroupService> logger)
        {
            _groupRepository = groupRepository;
            _logger = logger;
        }

        public async Task<List<GroupResponseDto>> GetAllGroupsAsync()
        {
            var groups = await _groupRepository.GetAllGroupsAsync();
            _logger.LogInformation("{Count} groupes trouvés", groups.Count);
            return groups.Select(GroupMapper.ToDto).ToList();
        }
        public async Task<GroupResponseDto> GetGroupByIdAsync(string id)
        {
            var group = await _groupRepository.GetGroupByIdAsync(id);
            if (group is null)
            {
                _logger.LogWarning("Groupe {Id} non trouvé", id);
                throw GroupNotFoundException.ById(id);
            }
            _logger.LogInformation("Groupe {Id} trouvé : {Label}", group.Id, group.Label);
            return GroupMapper.ToDto(group);
        }
        public async Task<GroupResponseDto> GetGroupByLabelAsync(string label)
        {
            var group = await _groupRepository.GetGroupByLabelAsync(label);
            if (group is null)
            {
                _logger.LogWarning("Groupe avec le label {Label} non trouvé", label);
                throw GroupNotFoundException.ByLabel(label);
            }
            _logger.LogInformation("Groupe {Label} trouvé", group.Label);
            return GroupMapper.ToDto(group);
        }
        public async Task<GroupResponseDto> CreateGroupAsync(CreateGroupDto groupDto)
        {
            var group = GroupMapper.ToEntity(groupDto);
            var createdGroup = await _groupRepository.CreateGroupAsync(group);
            _logger.LogInformation("Group {Id} crée avec succès", createdGroup.Id);
            return GroupMapper.ToDto(createdGroup);
        }
    }
}
