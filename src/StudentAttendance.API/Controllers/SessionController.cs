using Microsoft.AspNetCore.Mvc;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Absence;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Requests;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Response;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.API.Controllers;

[Route("api/sessions")]
[ApiController]
[Produces("application/json")]
public class SessionController : ControllerBase
{
    private readonly ISessionsService _sessionsService;
    private readonly ILogger<SessionController> _logger;

    public SessionController(ISessionsService sessionsService, ILogger<SessionController> logger)
    {
        _sessionsService = sessionsService ?? throw new ArgumentNullException(nameof(sessionsService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Récupère toutes les séances
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<SessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<SessionResponse>>> GetAllSessionsAsync()
    {
        try
        {
            var sessions = await _sessionsService.GetAllSessionsAsync();
            return Ok(sessions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de toutes les séances");
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors du traitement de votre demande.");
        }
    }

    /// <summary>
    /// Récupère une séance par son identifiant
    /// </summary>
    [HttpGet("{id}", Name = "GetSessionById")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SessionResponse>> GetSessionByIdAsync(string id)
    {
        try
        {
            var session = await _sessionsService.GetSessionsByIdAsync(id);
            if (session == null)
                return NotFound($"La séance avec l'identifiant '{id}' est introuvable.");

            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de la séance {SessionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors du traitement de votre demande.");
        }
    }

    /// <summary>
    /// Récupère les séances d'un professeur
    /// </summary>
    [HttpGet("teacher/{teacherId}")]
    [ProducesResponseType(typeof(List<SessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<SessionResponse>>> GetSessionByTeacherIdAsync(string teacherId)
    {
        try
        {
            var sessions = await _sessionsService.GetSessionsByTeacherIdAsync(teacherId);
            return Ok(sessions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des séances du professeur {TeacherId}", teacherId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors du traitement de votre demande.");
        }
    }

    /// <summary>
    /// Récupère les séances d'un groupe
    /// </summary>
    [HttpGet("group/{group}")]
    [ProducesResponseType(typeof(List<SessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<SessionResponse>>> GetSessionsByGroupName(string group)
    {
        try
        {
            var sessions = await _sessionsService.GetSessionsByGroupName(group);
            return Ok(sessions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des séances du groupe {GroupName}", group);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors du traitement de votre demande.");
        }
    }

    /// <summary>
    /// Récupère les étudiants d'une séance
    /// </summary>
    [HttpGet("{sessionId}/students")]
    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<User>>> GetStudentsBySessionIdAsync(string sessionId)
    {
        try
        {
            var students = await _sessionsService.GetStudentsBySessionIdAsync(sessionId);
            if (students == null || !students.Any())
                return NotFound($"Aucun étudiant trouvé pour la séance '{sessionId}'.");

            return Ok(students);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des étudiants de la séance {SessionId}", sessionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors du traitement de votre demande.");
        }
    }

    /// <summary>
    /// Récupère le professeur d'une séance
    /// </summary>
    [HttpGet("{sessionId}/professor")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> GetProfessorBySessionIdAsync(string sessionId)
    {
        try
        {
            var teacherId = await _sessionsService.GetProfessurBySessionIdAsync(sessionId);
            if (teacherId == null)
                return NotFound($"Aucun professeur trouvé pour la séance '{sessionId}'.");

            return Ok(teacherId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du professeur de la séance {SessionId}", sessionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors du traitement de votre demande.");
        }
    }

    /// <summary>
    /// Récupère les présences d'un étudiant avec les détails de chaque séance
    /// </summary>
    [HttpGet("student/{studentId}/absences")]
    [ProducesResponseType(typeof(List<StudentAttendanceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStudentAttendance([FromRoute] string studentId, CancellationToken ct)
    {
        try
        {
            var result = await _sessionsService.GetMyAttendanceAsync(studentId, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des présences de l'étudiant {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors du traitement de votre demande.");
        }
    }

    /// <summary>
    /// Crée une nouvelle séance
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SessionResponse>> CreateSessionAsync([FromBody] CreateSessionRequest sessionrequest)
    {
        if (sessionrequest == null) return BadRequest("Les données de la séance sont requises.");

        try
        {
            var createdSession = await _sessionsService.CreateSessionsAsync(sessionrequest);
            return CreatedAtRoute("GetSessionById", new { id = createdSession.Id }, createdSession);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création de la séance");
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors du traitement de votre demande.");
        }
    }

    /// <summary>
    /// Met à jour une séance existante
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SessionResponse?>> UpdateSessionAsync(string id, [FromBody] UpdateSessionRequest sessionrequest)
    {
        if (sessionrequest == null) return BadRequest("Les données de la séance sont requises.");

        try
        {
            var updatedSession = await _sessionsService.UpdateSessionsAsync(id, sessionrequest);
            if (updatedSession == null) return NotFound($"La séance avec l'identifiant '{id}' est introuvable.");
            return Ok(updatedSession);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour de la séance {SessionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors du traitement de votre demande.");
        }
    }

    /// <summary>
    /// Supprime une séance
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteSessionsAsync(string id)
    {
        try
        {
            var deleted = await _sessionsService.DeleteSessionsAsync(id);
            if (deleted) return NoContent();
            return NotFound($"La séance avec l'identifiant '{id}' est introuvable.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression de la séance {SessionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors du traitement de votre demande.");
        }
    }

    /// <summary>
    /// Justifie une absence d'un étudiant dans une séance
    /// La séance doit être validée et le statut doit être ABSENT ou LATE
    /// </summary>
    [HttpPut("{sessionId}/absences/{studentId}/justify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> JustifyAbsenceAsync(string sessionId, string studentId)
    {
        try
        {
            var result = await _sessionsService.JustifyAbsenceAsync(sessionId, studentId);
            if (!result) return NotFound($"Aucune absence trouvée pour l'étudiant '{studentId}' dans la séance '{sessionId}'.");
            return Ok("Absence justifiée avec succès.");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la justification de l'absence de l'étudiant {StudentId} dans la séance {SessionId}", studentId, sessionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors du traitement de votre demande.");
        }
    }

    /// <summary>
    /// Met à jour le statut d'une absence d'un étudiant dans une séance
    /// </summary>
    [HttpPatch("{sessionId}/absences/{studentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateAbsenceStatusAsync(string sessionId, string studentId, [FromBody] UpdateAbsenceStatusRequest request)
    {
        if (request == null) return BadRequest("Les données de la requête sont requises.");

        try
        {
            var result = await _sessionsService.UpdateAbsenceStatusAsync(sessionId, studentId, request);
            if (!result) return NotFound($"Aucune absence trouvée pour l'étudiant '{studentId}' dans la séance '{sessionId}'.");
            return Ok("Statut de l'absence mis à jour avec succès.");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour du statut d'absence de l'étudiant {StudentId} dans la séance {SessionId}", studentId, sessionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors du traitement de votre demande.");
        }
    }

    /// <summary>
    /// Met à jour le statut de plusieurs absences en une seule opération
    /// </summary>
    [HttpPatch("{sessionId}/absences")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateAbsencesBulkAsync(string sessionId, [FromBody] List<UpdateAbsencesBulkItem> items)
    {
        if (items == null || !items.Any()) return BadRequest("La liste des absences est requise.");

        try
        {
            var result = await _sessionsService.UpdateAbsencesBulkAsync(sessionId, items);
            if (!result) return NotFound($"Aucune absence mise à jour pour la séance '{sessionId}'.");
            return Ok("Absences mises à jour avec succès.");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour en masse des absences pour la séance {SessionId}", sessionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors du traitement de votre demande.");
        }
    }

    /// <summary>
    /// Valide une séance et marque les absences des étudiants
    /// Seul le professeur de la séance peut la valider
    /// </summary>
    [HttpPost("{sessionId}/validate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ValidateSessionAndMarkAbsences(
        [FromRoute] string sessionId,
        [FromBody] MarkAbsencesRequest request)
    {
        try
        {
            await _sessionsService.ValidateAndMarkAsync(sessionId, request);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation de la séance {SessionId}", sessionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors du traitement de votre demande.");
        }
    }
}