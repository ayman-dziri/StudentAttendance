using StudentAttendance.src.StudentAttendance.Application.DTOs.Auth;

namespace StudentAttendance.src.StudentAttendance.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto login, CancellationToken cancellationToken = default);
    }
}
