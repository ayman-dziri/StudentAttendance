using StudentAttendance.src.StudentAttendance.Domain.Enums;

namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Absence;

public record UpdateAbsencesBulkItem(
    string StudentId,
    StatusPresence Status
);