using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.GridFS;
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
        public async Task<IActionResult> GetAllGroups()
        {
            var groups = await _groupService.GetAllGroupsAsync();
            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupById(string id)
        {
            try
            {
                var group = await _groupService.GetGroupByIdAsync(id);
                return Ok(group);
            }
            catch (GroupNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("by-label/{label}")]
        public async Task<IActionResult> GetGroupByLabel(string label)
        {
            try
            {
                var group = await _groupService.GetGroupByLabelAsync(label);
                return Ok(group);
            }
            catch (GroupNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto groupDto)
        {
            var createdGroup = await _groupService.CreateGroupAsync(groupDto);
            return CreatedAtAction(nameof(GetGroupById), new { id = createdGroup.Id }, createdGroup);
        }
    }
}
