
using StudentAttendance.src.StudentAttendance.Application.DTOs.Auth;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Auth.Requests;

namespace StudentAttendance.src.StudentAttendance.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto login, CancellationToken cancellationToken = default);
        Task LogoutAsync(string userId, CancellationToken ct = default);
        Task ChangePasswordAsync(string userId , ChangePasswordRequest request , CancellationToken cancellationToken = default);
    } 
}

