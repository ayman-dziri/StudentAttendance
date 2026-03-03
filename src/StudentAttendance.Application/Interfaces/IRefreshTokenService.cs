
using StudentAttendance.src.StudentAttendance.Application.DTOs.Auth;

namespace StudentAttendance.src.StudentAttendance.Application.Interfaces
{
	public interface IRefreshTokenService
	{		
		Task<TokenResponse> RefreshAsync(string refreshToken, CancellationToken ct = default);
		Task InvalidateAllAsync(string userId, CancellationToken ct = default);
	}
}
