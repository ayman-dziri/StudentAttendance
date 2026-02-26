using MongoDB.Driver;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Data;



    public sealed class MongoDbContext
    {
    public IMongoDatabase Database { get; }

    public MongoDbContext(MongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        Database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name) =>
        Database.GetCollection<T>(name);
}

