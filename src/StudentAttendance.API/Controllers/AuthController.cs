using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;

namespace StudentAttendance.src.StudentAttendance.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController
{
    //private readonly IAuthService authService;

    //public AuthController(IAuthService authService)
    //{
    //    this.authService = authService;
    //}

    //[HttpPost("login")]
    //public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var response = await authService.LoginAsync(request, cancellationToken);
    //        return Ok(response);
    //    }
    //    catch (UnauthorizedAccessException ex)
    //    {
    //        return UnauthorizedAccessException(new { message = ex.Message });
    //    }
    //}

}
