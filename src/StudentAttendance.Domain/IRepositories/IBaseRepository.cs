namespace StudentAttendance.src.StudentAttendance.Domain.IRepositories
{
    public interface IBaseRepository<T>
    {
        Task<T?> GetByIdAsync(string id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);
    }
}
