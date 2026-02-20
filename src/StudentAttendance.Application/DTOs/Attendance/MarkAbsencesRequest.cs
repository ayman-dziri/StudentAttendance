using StudentAttendance.src.StudentAttendance.Domain.Enums;

namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Attendance
{
    public record StudentAbsenceMarkDto(
        string StudentId,
        StatusPresence Status,
        DateTime? JustificationDate
    );

    public record MarkAbsencesRequest(List<StudentAbsenceMarkDto> Marks);

}
