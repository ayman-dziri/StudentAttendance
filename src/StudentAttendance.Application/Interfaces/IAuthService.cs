using StudentAttendanceV2.src.StudentAttendance.Application.DTOs.Auth.Requests;

namespace StudentAttendanceV2.src.StudentAttendance.Application.Interfaces;

    public interface IAuthService
    {
        Task ChangePasswordAsync(string userId , ChangePasswordRequest request , CancellationToken cancellationToken = default);
    }
