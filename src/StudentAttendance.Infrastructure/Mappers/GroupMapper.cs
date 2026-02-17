using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Infrastructure.Documents;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Mappers
{
    public class GroupMapper
    {
        public static GroupDocument ToDocument(Group u) => new()
        {
            Id = u.Id,
            Label = u.Label,
            CreatedAt = u.CreatedAt
        };

        public static Group ToDomain(GroupDocument d) => new()
        {
            Id = d.Id,
            Label = d.Label,
            CreatedAt = d.CreatedAt
        };
    }
}
