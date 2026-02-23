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

    // GET api/Session
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
            _logger.LogError(ex, "An error occurred while retrieving sessions.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
    }

    // GET api/Session/{id}
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
                return NotFound($"Session with ID {id} not found.");

            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving session with ID {SessionId}.", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
    }

    // GET api/Session/teacher/{teacherId}
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
            _logger.LogError(ex, "An error occurred while retrieving session for teacher ID {TeacherId}", teacherId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
    }

    // GET api/Session/group/{group}
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
            _logger.LogError(ex, "An error occurred while retrieving session for group name {Group}.", group);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
    }

    // GET api/Session/{sessionId}/students
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
                return NotFound($"No students found for session {sessionId}");

            return Ok(students);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting students for session {SessionId}", sessionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving students.");
        }
    }

    // GET api/Session/{sessionId}/professor
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
                return NotFound($"No professor found for session {sessionId}");

            return Ok(teacherId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting professor for session {SessionId}", sessionId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the professor.");
        }
    }

    // POST api/Session/create
    [HttpPost("create")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SessionResponse>> CreateSessionAsync([FromBody] CreateSessionRequest sessionrequest)
    {
        if (sessionrequest == null) return BadRequest("Session data is required");

        try
        {
            var createdSession = await _sessionsService.CreateSessionsAsync(sessionrequest);
            return CreatedAtRoute("GetSessionById",new { id = createdSession.Id },createdSession);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating session");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
        }
    }

    // PUT api/Session/update/{id}
    [HttpPut("update/{id}")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
        }
    }

    // DELETE api/Session/delete/{id}
    [HttpDelete("delete/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteSessionsAsync(string id)
    {
        try
        {
            var deleted = await _sessionsService.DeleteSessionsAsync(id);
            if (deleted) return NoContent();
            return NotFound($"The session with Id {id} was not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting session with ID: {SessionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the session");
        }
    }
}