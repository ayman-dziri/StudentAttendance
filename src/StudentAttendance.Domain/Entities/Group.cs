using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StudentAttendance.src.StudentAttendance.Domain.Entities
{
    public class Group
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("label")]
        public string Label { get; set; } = null!;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
