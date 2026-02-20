namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Requests;

    public class CreateSessionRequest
    {
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string TeacherId { get; set; } = null!;
    public string Group { get; set; } = null!;

    public bool IsValidated { get; set; } = false ;
}

