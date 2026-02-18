namespace StudentAttendance.src.StudentAttendance.Domain.Entities
{
    public class Session
    {

        public string Id { get; set; } = null!;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string TeacherId { get; set; } = null!;

        public string Group { get; set; } = null!;
        public bool IsValidated { get; set; } = false; // ✅ new

    }
}
