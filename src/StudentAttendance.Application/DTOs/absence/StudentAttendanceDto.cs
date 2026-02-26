using StudentAttendance.src.StudentAttendance.Domain.Enums;

namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Absence;

public record StudentAttendanceDto(
    string SessionId,
    DateTime StartTime,
    DateTime EndTime,
    string Group,
    string TeacherId,
    StatusPresence Status,
    DateTime? JustificationDate
);