using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using StudentAttendance.src.StudentAttendance.Domain.Enums;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Documents
{
    public class AbsenceDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("status")]
        public StatusPresence Status { get; set; }

        [BsonElement("studentId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string StudentId { get; set; } = null!;

        [BsonElement("sessionId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SessionId { get; set; } = null!;

        [BsonElement("justificationDate")]
        public DateTime? JustificationDate { get; set; }
    }
}
