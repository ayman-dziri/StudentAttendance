using Microsoft.AspNetCore.Authorization;
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
            var group = await _groupService.GetGroupByIdAsync(id, cancellationToken);
            return Ok(group);
        }

        [HttpGet("by-label/{label}")]
        public async Task<IActionResult> GetGroupByLabel(string label, CancellationToken cancellationToken = default)
        {
            var group = await _groupService.GetGroupByLabelAsync(label, cancellationToken);
            return Ok(group);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto groupDto, CancellationToken cancellationToken = default)
        {
            var createdGroup = await _groupService.CreateGroupAsync(groupDto, cancellationToken);
            return CreatedAtAction(nameof(GetGroupById), new { id = createdGroup.Id }, createdGroup);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(string id, [FromBody] UpdateGroupDto groupDto, CancellationToken cancellationToken = default)
        {
            await _groupService.UpdateGroupAsync(id, groupDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(string id, CancellationToken cancellationToken = default)
        {
            await _groupService.DeleteGroupAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
