using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StudentAttendance.src.StudentAttendance.Application.DTOs.user;
using StudentAttendance.src.StudentAttendance.Application.Interfaces;
using StudentAttendance.src.StudentAttendance.Application.Mappers;
using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest userRequest, CancellationToken ct)
        {
            await _userService.CreateUserAsync(userRequest, ct);

            return Created();
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(CancellationToken ct)
        {
            var users = await _userService.GetAllUsersAsync(ct);

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] string id , CancellationToken ct)
        {
            var user = await _userService.GetByIdAsync(id);

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpadateUser([FromBody] UpdateUserRequest userRequest,
                                                     [FromRoute] string id,
                                                     CancellationToken ct)
        {
            await _userService.UpdateUserAsync(id, userRequest, ct);
            return Ok(userRequest);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute]string id, CancellationToken ct)
        {
            await _userService.DeleteUserAsync(id, ct);
            return Ok();
        }

        [HttpGet("users/{groupId}")]
        public async Task<IActionResult> GetUsersByGroup([FromRoute] string groupId, CancellationToken ct)
        {
            var users = await _userService.GetStudentsByGroupAsync(groupId, ct);

            return Ok(users);
        }
    }
}
