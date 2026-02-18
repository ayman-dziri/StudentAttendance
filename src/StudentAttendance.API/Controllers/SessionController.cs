using Microsoft.AspNetCore.Mvc;
using StudentAttendance.src.StudentAttendance.Application.Intefaces;
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
    [ProducesResponseType(typeof(List<Session>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]


    public async Task<ActionResult<List<Session>>> GetAllSessionsAsync()
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
    [ProducesResponseType(typeof(Session), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<ActionResult<Session>> GetSessionByIdAsync(string id)
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


    [HttpGet("/sessions/{teacherId}")]
    [ProducesResponseType(typeof(Session), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<ActionResult<List<Session>>> GetSessionByTeacherIdAsync(string teachedId)
    {
        try
        {
            var session = await _sessionsService.GetSessionsByTeacherIdAsync(teachedId);
            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while retrieving session for teacher ID {teachedId}.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
    }


    [HttpGet("/sessions/{group}")]
    [ProducesResponseType(typeof(Session), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<ActionResult<List<Session>>> GetSessionsByGroupName(string group)
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

    [HttpPost("/sessions/create")]
    [ProducesResponseType(typeof(Session), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<ActionResult<Session>> CreateSessionsAsync([FromBody] Session session)
    {
        try
        {
            if (session == null)
            {
                _logger.LogWarning("Received null session object for creation");
                return BadRequest("Session data is required");
            }


            var createdSession = await _sessionsService.CreateSessionsAsync(session);
            return CreatedAtAction(
                nameof(GetSessionByIdAsync),
                new { id = createdSession.Id },
                createdSession
                );
        }


        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating session: {SessionId}", session?.Id);
            return StatusCode(500, "An error occurred while creating the session");
            return BadRequest(ex.Message); //erreur 400 si les données de la session sont invalides
        }
    }


    [HttpPut("/sessions/update/{id}")]
    [ProducesResponseType(typeof(Session), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Session?>> UpdateSessionsAsync(string id, [FromBody] Session session)
    {
        try
        {
            if (session == null)
            {
                return BadRequest("Session data is required");
            }

            var updatedSession = await _sessionsService.UpdateSessionsAsync(id, session);

            if (updatedSession == null)
            {
                return NotFound($"The session with Id {id} was not found");
            }
            else
            {
                return Ok(updatedSession);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating session with ID: {SessionId}", id);
            return StatusCode(500, "An error occurred while updating the session");
        }
    }



    [HttpDelete("/sessions/delete/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]


    public async Task<ActionResult<Session>> DeleteSessionsAsync(string id)
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
