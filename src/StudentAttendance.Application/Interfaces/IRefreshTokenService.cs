namespace StudentAttendanceV2.src.StudentAttendance.Application.Interfaces;

    public interface IRefreshTokenService
    {
         Task InvalidateAllAsync(string userId, CancellationToken ct = default);
    }
