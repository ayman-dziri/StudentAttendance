using Microsoft.AspNetCore.Mvc;
using StudentAttendance.src.StudentAttendance.Application.DTOs.absence;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Application.Mappers;

namespace StudentAttendance.src.StudentAttendance.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AbsenceController : ControllerBase
{
    private readonly IAbsenceService _absenceService;

    public AbsenceController(IAbsenceService absenceService)
    {
        _absenceService = absenceService;
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
}