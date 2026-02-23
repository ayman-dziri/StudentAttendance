using Microsoft.AspNetCore.Identity.Data;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Auth.Responses;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;

namespace StudentAttendance.src.StudentAttendance.Application.Services;

    public class AuthService : IAuthService
    {
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    
    public AuthService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request , CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid Credentials or Account Desactivated");
        }

        var paswwordok = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        if(!paswwordok)
        {
            throw new UnauthorizedAccessException("Incorrect Password");
        }

        var (token, exp) = _tokenService.CreateAccessToken(user.Id, user.Email, user.Role.ToString());
        return new AuthResponse(token, exp);

    }
}

