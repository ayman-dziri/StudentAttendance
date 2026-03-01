using Microsoft.Extensions.Options;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Auth;
using StudentAttendance.src.StudentAttendance.Application.Exceptions;
using StudentAttendance.src.StudentAttendance.Application.Interfaces;
using StudentAttendance.src.StudentAttendance.Domain.Auth;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces;
using StudentAttendance.src.StudentAttendance.Domain.Repositories;
using StudentAttendance.src.StudentAttendance.Infrastructure.Auth;
using System.Security.Claims;

namespace StudentAttendance.src.StudentAttendance.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenProvider jwtTokenProvider, IOptions<JwtOptions> jwtOptions, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenProvider = jwtTokenProvider;
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
        }
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto login, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByEmailAsync(login.Email, cancellationToken);
            if (user is null)
            {
                _logger.LogWarning("Email invalide");
                throw new InvalidCredentialsException();
            }
            var verifa = _passwordHasher.Verify(login.Password, user.Password);
            if (!verifa)
            {
                _logger.LogWarning("Password invalide");
                throw new InvalidCredentialsException();
            }
            var claims = new Dictionary<string, string>
            {
                [ClaimTypes.Role] = user.Role.ToString()
            };
            JwtUserDescriptor jwtuser = new JwtUserDescriptor(user.Id, user.Email, claims);
            string token = _jwtTokenProvider.GenerateToken(jwtuser);
            return new LoginResponseDto(token, DateTime.UtcNow.AddMinutes(_jwtOptions.ExpMinuts));
        }
    }
}