namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Response;

    public class SessionResponse
    {
    public string Id { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string TeacherId { get; set; } = null!;

    public string Group { get; set; } = null!;

    public Boolean Statut { get; set; } = false;
}
