using Microsoft.AspNetCore.Mvc;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Group;
using StudentAttendance.src.StudentAttendance.Application.Exceptions;
using StudentAttendance.src.StudentAttendance.Application.Interfaces;

namespace StudentAttendance.src.StudentAttendance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGroups(CancellationToken cancellationToken = default)
        {
            var groups = await _groupService.GetAllGroupsAsync(cancellationToken);
            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupById(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var group = await _groupService.GetGroupByIdAsync(id, cancellationToken);
                return Ok(group);
            }
            catch (GroupNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("by-label/{label}")]
        public async Task<IActionResult> GetGroupByLabel(string label, CancellationToken cancellationToken = default)
        {
            try
            {
                var group = await _groupService.GetGroupByLabelAsync(label, cancellationToken);
                return Ok(group);
            }
            catch (GroupNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto groupDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var createdGroup = await _groupService.CreateGroupAsync(groupDto, cancellationToken);
                return CreatedAtAction(nameof(GetGroupById), new { id = createdGroup.Id }, createdGroup);
            }
            catch (DuplicateGroupException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(string id, [FromBody] UpdateGroupDto groupDto, CancellationToken cancellationToken = default)
        {
            try
            {
                await _groupService.UpdateGroupAsync(id, groupDto, cancellationToken);
                return NoContent();
            }
            catch (GroupNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DuplicateGroupException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                await _groupService.DeleteGroupAsync(id, cancellationToken);
                return NoContent();
            }
            catch (GroupNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
