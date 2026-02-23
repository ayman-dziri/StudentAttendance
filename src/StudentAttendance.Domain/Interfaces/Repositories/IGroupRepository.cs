using System.Threading;
using System.Threading.Tasks;
using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories
{
    public interface IGroupRepository
    {
        Task<Group?> GetByNameAsync(string label, CancellationToken ct = default);
        Task<Group> CreateAsync(Group group, CancellationToken ct = default);
    }
}

