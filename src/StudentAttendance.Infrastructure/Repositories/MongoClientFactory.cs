

using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
using StudentAttendance.src.StudentAttendance.Infrastructure.Interfaces;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Repositories;




public class MongoClientFactory : IMongoClientFactory
{
    protected readonly IMongoDatabase _database;

    protected readonly MongoClient _client;

    protected readonly IOptions<MongoDbSettings> _options;

    public MongoClientFactory(IOptions<MongoDbSettings> options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));

        var connectionString = _options.Value.ConnectionString
            ?? throw new InvalidOperationException("MongoDB ConnectionString is not configured");

        var database = _options.Value.DatabaseName
            ?? throw new InvalidOperationException("MongoDB Database is not configured");

        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(database);

    }

    public IMongoCollection<TDocument> GetMongoCollection<TDocument>(string collectionName) where TDocument : class, new()
    {
        return _database.GetCollection<TDocument>(collectionName);
    }
}

