using StudentAttendance.src.StudentAttendance.Application.DTOs.Auth;

namespace StudentAttendance.src.StudentAttendance.Application.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponse> RefreshAsync(string refreshToken, CancellationToken ct = default);
        Task LogoutAsync(string userId, CancellationToken ct = default);
       
    }
}