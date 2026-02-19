using Microsoft.AspNetCore.Mvc;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Requests;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Response;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.API.Controllers;

[Route("api/[controller]")]
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

    [HttpGet("/sessions")]
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
            _logger.LogError(ex, "An error occurred while retrieving sessions.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
    }

    [HttpGet("/sessions/{id}")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<ActionResult<SessionResponse>> GetSessionByIdAsync(string id)
    {
        try
        {
            var session = await _sessionsService.GetSessionsByIdAsync(id);
            if (session == null)
            {
                return NotFound($"Session with ID {id} not found.");
            }
            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while retrieving session with ID {id}.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
    }


    [HttpGet("/session/teacher/{teacherId}")]
    [ProducesResponseType(typeof(List<SessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<ActionResult<List<SessionResponse>>> GetSessionByTeacherIdAsync(string teacherId)
    {
        try
        {
            var session = await _sessionsService.GetSessionsByTeacherIdAsync(teacherId);
            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while retrieving session for teacher ID {teacherId}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
    }


    [HttpGet("/session/group/{group}")]
    [ProducesResponseType(typeof(List<SessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<ActionResult<List<SessionResponse>>> GetSessionsByGroupName(string group)
    {
        try
        {
            var session = await _sessionsService.GetSessionsByGroupName(group);
            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while retrieving session for group name {group}.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
    }


    [HttpGet("/sessions/{sessionId}/students")]
    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<ActionResult<List<User>>> GetStudentsBySessionIdAsync(string sessionId)
    {
        try
        {
            var students = await _sessionsService.GetStudentsBySessionIdAsync(sessionId);
            if (students == null || !students.Any())
                return NotFound($"No students found for session {sessionId}");

            return Ok(students);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting students for session {SessionId}", sessionId);
            return StatusCode(500, "An error occurred while retrieving students.");
        }
    }


    [HttpGet("/sessions/{sessionId}/professor")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<User>> GetProfessorBySessionIdAsync(string sessionId)
{
    try
    {
        var professor = await _sessionsService.GetProfessurBySessionIdAsync(sessionId);
        if (professor == null)
            return NotFound($"No professor found for session {sessionId}");

        return Ok(professor);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting professor for session {SessionId}", sessionId);
        return StatusCode(500, "An error occurred while retrieving the professor.");
    }
}







    [HttpPost("/sessions/create")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<ActionResult<SessionResponse>> CreateSessionAsync([FromBody] CreateSessionRequest sessionrequest)
    {
        if (sessionrequest == null) return BadRequest("Session data is required");

        try
        {
            var createdSession = await _sessionsService.CreateSessionsAsync(sessionrequest);
            return CreatedAtAction(nameof(GetSessionByIdAsync), new { id = createdSession.Id }, createdSession);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating session");
            return StatusCode(500, "An error occurred while creating the session");
        }
    }


    [HttpPut("/sessions/update/{id}")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SessionResponse?>> UpdateSessionAsync(string id, [FromBody] UpdateSessionRequest sessionrequest)
    {
        if (sessionrequest == null) return BadRequest("Session data is required");

        try
        {
            var updatedSession = await _sessionsService.UpdateSessionsAsync(id, sessionrequest);
            if (updatedSession == null) return NotFound($"Session with ID {id} not found");
            return Ok(updatedSession);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating session with ID {SessionId}", id);
            return StatusCode(500, "An error occurred while updating the session");
        }
    }


    [HttpDelete("/sessions/delete/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]


    public async Task<ActionResult> DeleteSessionsAsync(string id)
    {
        try
        {
            var deleted = await _sessionsService.DeleteSessionsAsync(id);
            if (deleted)
            {
                return NoContent();
            }
            else
            {
                return NotFound($"The session with Id {id} was not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting session with ID: {SessionId}", id);
            return StatusCode(500, "An error occurred while deleting the session");
        }
    }
}
