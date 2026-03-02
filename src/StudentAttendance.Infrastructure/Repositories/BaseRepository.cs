<<<<<<< HEAD
using MongoDB.Driver;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
=======
using StudentAttendance.src.StudentAttendance.Domain.IRepositories;
>>>>>>> origin/feature/scrum-12-attendance-validation

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Repositories
{
<<<<<<< HEAD
    protected readonly IMongoCollection<T> _collection;

    public BaseRepository(MongoDbContext context, string collectionName)
=======
    public abstract class BaseRepository<T> : IBaseRepository<T>
>>>>>>> origin/feature/scrum-12-attendance-validation
    {
        public virtual Task<T?> GetByIdAsync(string id) => Task.FromResult<T?>(default);
        public virtual Task CreateAsync(T entity) => Task.CompletedTask;
        public virtual Task UpdateAsync(T entity) => Task.CompletedTask;
        public virtual Task DeleteAsync(string id) => Task.CompletedTask;
    }
}
