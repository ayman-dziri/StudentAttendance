using Microsoft.AspNetCore.Identity.Data;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Auth.Responses;

namespace StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;

    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }

