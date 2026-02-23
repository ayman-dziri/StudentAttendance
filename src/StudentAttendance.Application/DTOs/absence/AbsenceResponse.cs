using StudentAttendance.src.StudentAttendance.Domain.Enums;

namespace StudentAttendance.src.StudentAttendance.Application.DTOs.absence;

/// <summary>
/// Réponse renvoyée pour une absence 
/// </summary>
public class AbsenceResponse
{
    public string Id { get; set; } = null!;
    public string StudentId { get; set; } = null!;
    public string SessionId { get; set; } = null!;
    public StatusPresence Status { get; set; }
    public DateTime? JustificationDate { get; set; }
}