namespace StudentAttendance.src.StudentAttendance.Application.DTOs.absence;

/// <summary>
/// Réponse renvoyée pour une absence 
/// </summary>
public class AbsenceResponse
{
    public string Id { get; set; } = null!;
    public string StudentId { get; set; } = null!;
    public string SessionId { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime? JustificationDate { get; set; }
}