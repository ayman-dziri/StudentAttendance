namespace StudentAttendance.src.StudentAttendance.Domain.Entities
{
    public class Group
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
