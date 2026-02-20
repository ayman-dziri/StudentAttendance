using StudentAttendance.src.StudentAttendance.Application.DTOs.Group;
using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Application.Mappers
{
    public class GroupMapper
    {
        public static Group ToEntity(CreateGroupDto dto) => new()
        {
            Id = string.Empty,
            Label = dto.Label,
            CreatedAt = DateTime.UtcNow 
        };

        public static GroupResponseDto ToDto(Group group) => new()
        {
            Id = group.Id,
            Label = group.Label,
            CreatedAt = group.CreatedAt
        };
    }
}
