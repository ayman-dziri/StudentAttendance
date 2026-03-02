using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Auth;
using StudentAttendance.src.StudentAttendance.Application.Interfaces;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces;
using StudentAttendance.src.StudentAttendance.Domain.Auth;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Auth;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenProvider _jwtTokenProvider;
    public sealed record SeedRefreshTokenRequest(string Email);
    //public AuthController(IAuthService auth, IJwtTokenProvider jwtTokenProvider ) => _auth = auth, _jwtTokenProvider = jwtTokenProvider;

    public AuthController(IAuthService authService, IJwtTokenProvider jwtTokenProvider)
    {
        _authService = authService;
        _jwtTokenProvider = jwtTokenProvider;
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _authService.RefreshAsync(request.RefreshToken, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, detail = ex.ToString() });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        await _authService.LogoutAsync(userId, ct);
        return NoContent();
    }
}