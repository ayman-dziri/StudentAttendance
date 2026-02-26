using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using StudentAttendance.src.StudentAttendance.Domain.Enums;

namespace StudentAttendance.src.StudentAttendance.Domain.Entities
{
    public class Absence
    {

        public string Id { get; set; } = null!;

        public StatusPresence Status { get; set; }

        public string StudentId { get; set; } = null!;

        public DateTime? JustificationDate { get; set; }
    }
}
