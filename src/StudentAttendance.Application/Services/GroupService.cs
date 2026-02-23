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

        public async Task<List<GroupResponseDto>> GetAllGroupsAsync(CancellationToken cancellationToken = default)
        {
            var groups = await _groupRepository.GetAllGroupsAsync(cancellationToken);
            _logger.LogInformation("{Count} groupes trouvés", groups.Count);
            return groups.Select(GroupMapper.ToDto).ToList();
        }
        public async Task<GroupResponseDto> GetGroupByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var group = await _groupRepository.GetGroupByIdAsync(id, cancellationToken);
            if (group is null)
            {
                _logger.LogWarning("Groupe {Id} non trouvé", id);
                throw GroupNotFoundException.ById(id);
            }
            _logger.LogInformation("Groupe {Id} trouvé : {Label}", group.Id, group.Label);
            return GroupMapper.ToDto(group);
        }
        public async Task<GroupResponseDto> GetGroupByLabelAsync(string label, CancellationToken cancellationToken = default)
        {
            var group = await _groupRepository.GetGroupByLabelAsync(label, cancellationToken);
            if (group is null)
            {
                _logger.LogWarning("Groupe avec le label {Label} non trouvé", label);
                throw GroupNotFoundException.ByLabel(label);
            }
            _logger.LogInformation("Groupe {Label} trouvé", group.Label);
            return GroupMapper.ToDto(group);
        }
        public async Task<GroupResponseDto> CreateGroupAsync(CreateGroupDto groupDto, CancellationToken cancellationToken = default)
        {
            var exists = await _groupRepository.ExistsByLabelAsync(groupDto.Label, cancellationToken);
            if (exists)
            {
                _logger.LogWarning("Création refusée, groupe avec label {Label} existe déjà", groupDto.Label);
                throw new DuplicateGroupException(groupDto.Label);
            }
            var group = GroupMapper.ToEntity(groupDto);
            var createdGroup = await _groupRepository.CreateGroupAsync(group, cancellationToken);
            _logger.LogInformation("Group {Id} crée avec succès", createdGroup.Id);
            return GroupMapper.ToDto(createdGroup);
        }
        public async Task<bool> UpdateGroupAsync(string id, UpdateGroupDto groupDto, CancellationToken cancellationToken = default)
        {
            var existingGroup = await _groupRepository.GetGroupByIdAsync(id, cancellationToken);
            if (existingGroup is null)
            {
                _logger.LogWarning("Mise à jour impossible, groupe {Id} non trouvé", id);
                throw GroupNotFoundException.ById(id);
            }
            var groupWithSameLabel = await _groupRepository.GetGroupByLabelAsync(groupDto.Label, cancellationToken);
            if (groupWithSameLabel != null)
            {
                _logger.LogWarning("Mise à jour impossible, label {Label} déjà existe", groupDto.Label);
                throw new DuplicateGroupException(groupDto.Label);  
            }
            existingGroup.Label = groupDto.Label;
            var result = await _groupRepository.UpdateGroupAsync(id, existingGroup, cancellationToken);
            if (!result)
            {
                _logger.LogError("Echec de modification");
                throw new Exception("Echec de modification");
            }
            _logger.LogInformation("Groupe {Id} modifié avec succès", id);
            return true;
        }
        public async Task<bool> DeleteGroupAsync(string id, CancellationToken cancellationToken = default)
        {
            var result = await _groupRepository.DeleteGroupAsync(id, cancellationToken);
            if (!result)
            {
                _logger.LogWarning("Suppression impossible, groupe {Id} non trouvé", id);
                throw GroupNotFoundException.ById(id);
            }
            _logger.LogInformation("Groupe {Id} supprimé avec succès", id);
            return true;
        }
    }
}
