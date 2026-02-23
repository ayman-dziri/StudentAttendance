namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Auth.Responses;

public record AuthResponse(string AccessToken, DateTime ExpiresAtUtc);

