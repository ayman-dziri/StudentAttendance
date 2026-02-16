using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StudentAttendance.src.StudentAttendance.Domain.Entities
{
    public class Session
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("startTime")]
        public DateTime StartTime { get; set; }

        [BsonElement("endTime")]
        public DateTime EndTime { get; set; }

        [BsonElement("teacherId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TeacherId { get; set; } = null!;

        [BsonElement("groupId")]
        public string Group { get; set; } = null!;
    }
}
