using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StudentAttendance.src.StudentAttendance.API.Constants;
using StudentAttendance.src.StudentAttendance.Application.DTOs.user;
using StudentAttendance.src.StudentAttendance.Application.Interfaces;
using StudentAttendance.src.StudentAttendance.Application.Mappers;
using StudentAttendance.src.StudentAttendance.Domain.Entities;

namespace StudentAttendance.src.StudentAttendance.API.Controllers;

[Route("api/users")]
[ApiController]
//[Authorize(Roles = Roles.Admin)]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
           

    public UserController(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        
    }

    /// <summary>
    /// Crée un nouvel utilisateur
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest userRequest, CancellationToken ct)
    {
        await _userService.CreateUserAsync(userRequest, ct);
        return Created();
    }

    /// <summary>
    /// Récupère tous les utilisateurs
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUsers(CancellationToken ct)
    {
        var users = await _userService.GetAllUsersAsync(ct);
        return Ok(users);
    }

    /// <summary>
    /// Récupère un utilisateur par son identifiant
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser([FromRoute] string id, CancellationToken ct)
    {
        var user = await _userService.GetByIdAsync(id);
        return Ok(user);
    }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id, CancellationToken ct)
        {
            await _userService.DeleteUserAsync(id, ct);
            return Ok();
        }
    /// <summary>
    /// Met à jour un utilisateur
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest userRequest,
                                                [FromRoute] string id,
                                                CancellationToken ct)
    {
        await _userService.UpdateUserAsync(id, userRequest, ct);
        return Ok(userRequest);
    }   


        
    /// <summary>
    /// Récupère les étudiants d'un groupe
    /// </summary>
    [HttpGet("group/{groupId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUsersByGroup([FromRoute] string groupId, CancellationToken ct)
    {
        var users = await _userService.GetStudentsByGroupAsync(groupId, ct);
        return Ok(users);
    }
}