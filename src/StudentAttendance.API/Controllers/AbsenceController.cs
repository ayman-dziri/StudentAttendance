using Microsoft.AspNetCore.Mvc;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Absence;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Application.Mappers;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;

namespace StudentAttendance.src.StudentAttendance.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AbsenceController : ControllerBase
{
    private readonly IAbsenceService _absenceService;
    private readonly ISessionsRepository _sessions;

    public AbsenceController(IAbsenceService absenceService, ISessionsRepository sessions)
    {
        _absenceService = absenceService;
        _sessions = sessions;
    }

    /// <summary>
    /// Récupère la liste des absences d'une séance
    /// </summary>
    [HttpGet("session/{sessionId}")]
    [ProducesResponseType(typeof(List<AbsenceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAbsencesBySession(string sessionId, CancellationToken cancellationToken)
    {
        var absences = await _absenceService.GetAbsencesBySessionAsync(sessionId, cancellationToken);
        var response = absences.Select(AbsenceMapper.ToResponse).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Justifie une absence (admin uniquement)
    /// </summary>
    [HttpPut("{absenceId}/justify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> JustifyAbsence(string absenceId, CancellationToken cancellationToken)
    {
        await _absenceService.JustifyAbsenceAsync(absenceId, cancellationToken);
        return Ok(new { message = "Absence justifiée avec succès" });
    }

    // PROF: Valider une séance + marquer absences
    [HttpPost("teachers/{teacherId}/sessions/{sessionId}/validate")]
    public async Task<IActionResult> ValidateSessionAndMarkAbsences(
        [FromRoute] string teacherId,
        [FromRoute] string sessionId,
        [FromBody] MarkAbsencesRequest request)
    {
        await _absenceService.ValidateAndMarkAsync(teacherId, sessionId, request);
        return NoContent(); // 204
    }

    // ETUDIANT: Consulter ses absences
    [HttpGet("students/{studentId}/absences")]
    public async Task<IActionResult> GetStudentAbsences([FromRoute] string studentId)
    {
        var result = await _absenceService.GetMyAbsencesAsync(studentId);
        return Ok(result);
    }

    // PROF: Consulter les détails d'une séance (optionnel)
    [HttpGet("sessions/{sessionId}")]
    public async Task<IActionResult> GetSession(string sessionId)
    {
        var s = await _sessions.GetSessionsByIdAsync(sessionId);
        if (s is null) return NotFound();
        return Ok(s);
    }
}