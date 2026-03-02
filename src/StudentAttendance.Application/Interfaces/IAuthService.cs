<<<<<<< HEAD
using StudentAttendance.src.StudentAttendance.Application.DTOs.Auth;
=======
﻿using StudentAttendance.src.StudentAttendance.Application.DTOs.Auth;
>>>>>>> origin/feature/scrum-28-user-login

namespace StudentAttendance.src.StudentAttendance.Application.Interfaces
{
    public interface IAuthService
    {
<<<<<<< HEAD
        Task<TokenResponse> RefreshAsync(string refreshToken, CancellationToken ct = default);
        Task LogoutAsync(string userId, CancellationToken ct = default);
       
    }
}
=======
        Task<LoginResponseDto> LoginAsync(LoginRequestDto login, CancellationToken cancellationToken = default);
    }
}
>>>>>>> origin/feature/scrum-28-user-login
