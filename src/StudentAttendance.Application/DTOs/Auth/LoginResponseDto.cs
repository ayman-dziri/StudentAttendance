namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Auth;

public record LoginResponseDto(string AccessToken, string RefreshToken,DateTime AccessTokenExpiresAtUtc,DateTime RefreshTokenExpiresAtUtc);