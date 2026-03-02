using StudentAttendance.src.StudentAttendance.Domain.Enums;

namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Attendance
{
    public record AbsenceDto(
        string Id,
        string SessionId,
        StatusPresence Status,
        DateTime? JustificationDate
    );
}
