using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAttendance.src.StudentAttendance.API.Constants;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Group;
using StudentAttendance.src.StudentAttendance.Application.Interfaces;

namespace StudentAttendance.src.StudentAttendance.API.Controllers;

[ApiController]
[Route("api/groups")]
[Authorize(Roles = Roles.Admin)]
public class GroupController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupController(IGroupService groupService)
    {
        _groupService = groupService ?? throw new ArgumentNullException(nameof(groupService));
    }

    /// <summary>
    /// Récupère tous les groupes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllGroups(CancellationToken cancellationToken = default)
    {
        var groups = await _groupService.GetAllGroupsAsync(cancellationToken);
        return Ok(groups);
    }

    /// <summary>
    /// Récupère un groupe par son identifiant
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupById(string id, CancellationToken cancellationToken = default)
    {
       var group = await _groupService.GetGroupByIdAsync(id, cancellationToken);
        return Ok(group);
    }

    /// <summary>
    /// Récupère un groupe par son label
    /// </summary>
    [HttpGet("by-label/{label}")]
    [ProducesResponseType(StatusCodes.Status200OK)] 
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupByLabel(string label, CancellationToken cancellationToken = default)
    {
        var group = await _groupService.GetGroupByLabelAsync(label, cancellationToken);
        return Ok(group);
    }

    /// <summary>
    /// Crée un nouveau groupe
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto groupDto, CancellationToken cancellationToken = default)
    {
        var createdGroup = await _groupService.CreateGroupAsync(groupDto, cancellationToken);
        return CreatedAtAction(nameof(GetGroupById), new { id = createdGroup.Id }, createdGroup);
    }

    /// <summary>
    /// Met à jour un groupe
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateGroup(string id, [FromBody] UpdateGroupDto groupDto, CancellationToken cancellationToken = default)
    {
        await _groupService.UpdateGroupAsync(id, groupDto, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Supprime un groupe
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGroup(string id, CancellationToken cancellationToken = default)
    {
        await _groupService.DeleteGroupAsync(id, cancellationToken);
        return NoContent();
    }
}