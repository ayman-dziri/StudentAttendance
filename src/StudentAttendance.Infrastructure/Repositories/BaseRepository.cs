using StudentAttendance.src.StudentAttendance.Domain.IRepositories;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T>
    {
        public virtual Task<T?> GetByIdAsync(string id) => Task.FromResult<T?>(default);
        public virtual Task CreateAsync(T entity) => Task.CompletedTask;
        public virtual Task UpdateAsync(T entity) => Task.CompletedTask;
        public virtual Task DeleteAsync(string id) => Task.CompletedTask;
    }
}
