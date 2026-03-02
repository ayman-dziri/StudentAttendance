using MongoDB.Driver;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Interfaces;

public interface IMongoClientFactory
{
    IMongoCollection<TDocument> GetMongoCollection<TDocument>(string collectionName) where TDocument : class, new();

}

