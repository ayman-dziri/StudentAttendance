namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Auth;

public record LoginResponseDto(string AccessToken, DateTime ExpiresAtUtc);