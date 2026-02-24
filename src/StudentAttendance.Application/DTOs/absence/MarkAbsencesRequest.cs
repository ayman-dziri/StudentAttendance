using StudentAttendance.src.StudentAttendance.Domain.Enums;

namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Absence;

public class MarkAbsencesRequest
{
    public List<StudentAbsenceMarkDto> Marks { get; set; } = new();
}

public class StudentAbsenceMarkDto
{
    public string AbsenceId { get; set; } = null!;
    public StatusPresence Status { get; set; }
    public DateTime? JustificationDate { get; set; } 
}