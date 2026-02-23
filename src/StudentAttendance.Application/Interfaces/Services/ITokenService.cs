namespace StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;

    

    public interface ITokenService
    {
        (string AccessToken, DateTime ExpiresAtUtc) CreateAccessToken(string userId, string email, string role);
    }

