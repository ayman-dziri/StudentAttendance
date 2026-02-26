using StudentAttendance.src.StudentAttendance.Application.DTOs.Absence;
using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.Application.Mappers;

/// <summary>
/// Mapping entre entité Absence et DTO de réponse
/// </summary>
public static class AbsenceMapper
{
    public static AbsenceResponse ToResponse(Absence absence) => new()
    {
        Id = absence.Id,
        StudentId = absence.StudentId,
        Status = absence.Status,
        JustificationDate = absence.JustificationDate
    };
}