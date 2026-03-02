namespace StudentAttendance.src.StudentAttendance.Application.DTOs.Auth
{
    public sealed record TokenResponse(
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpiresAtUtc,
        DateTime RefreshTokenExpiresAtUtc
    );
}