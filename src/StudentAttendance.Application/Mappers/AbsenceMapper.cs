using StudentAttendance.src.StudentAttendance.Application.DTOs.Absence;
using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Application.Mappers;

public static class AbsenceMapper
{
    public static AbsenceDto ToResponse(Absence absence)
        => new AbsenceDto(
            absence.Id,
            absence.SessionId,
            absence.Status,
            absence.JustificationDate
        );
}