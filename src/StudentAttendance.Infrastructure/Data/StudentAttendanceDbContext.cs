using MongoDB.Driver;
using StudentAttendance.src.StudentAttendance.Infrastructure.Configuration;
namespace StudentAttendance.src.StudentAttendance.Infrastructure.Data
{
    public sealed class StudentAttendanceDbContext
    {
        private readonly IMongoDatabase _database;

    public StudentAttendanceDbContext(MongoDbSettings settings)
    {
        if (settings is null) throw new ArgumentNullException(nameof(settings));

        if (string.IsNullOrWhiteSpace(settings.ConnectionString))
            throw new InvalidOperationException("MongoDB ConnectionString is not configured");

        if (string.IsNullOrWhiteSpace(settings.DatabaseName))
            throw new InvalidOperationException("MongoDB DatabaseName is not configured");

        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }

    /// <summary>
    /// Récupère une collection MongoDB par son nom
    /// </summary>
    /// <typeparam name="T">Type de l'entité</typeparam>
    /// <param name="collectionName">Nom de la collection</param>
    public IMongoCollection<T> GetCollection<T>(string collectionName) =>
        _database.GetCollection<T>(collectionName);
    }
}
