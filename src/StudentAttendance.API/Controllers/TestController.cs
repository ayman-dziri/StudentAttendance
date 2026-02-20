namespace StudentAttendance.src.StudentAttendance.API.Controllers;

using global::StudentAttendance.src.StudentAttendance.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;


 

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly IMongoClientFactory _mongoClientFactory;

    public TestController(IMongoClientFactory mongoClientFactory)
    {
        _mongoClientFactory = mongoClientFactory;
    }

    [HttpPost]
    public IActionResult CreateTestDocument()
    {
        var collection = _mongoClientFactory.GetMongoCollection<BsonDocument>("test");
        var document = new BsonDocument
        {
            { "name", "Test" },
            { "message", "MongoDB fonctionne !" }
        };
        collection.InsertOne(document);
        return Ok("Document créé avec succès !");
    }
}
