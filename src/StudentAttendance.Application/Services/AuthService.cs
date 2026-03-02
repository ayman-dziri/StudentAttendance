using StudentAttendance.src.StudentAttendance.Application.DTOs.Auth;
using StudentAttendance.src.StudentAttendance.Application.Exceptions;
using StudentAttendance.src.StudentAttendance.Application.Interfaces;
using StudentAttendance.src.StudentAttendance.Domain.Auth;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces;
using StudentAttendance.src.StudentAttendance.Domain.Repositories;
using System.Security.Claims;

namespace StudentAttendance.src.StudentAttendance.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenProvider jwtTokenProvider, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenProvider = jwtTokenProvider;
            _logger = logger;
        }
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto login, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByEmailAsync(login.Email, cancellationToken);
            if (user is null)
            {
                _logger.LogWarning("Echec de connexion pour {Email}", login.Email);
                throw new InvalidCredentialsException();
            }
            var isPasswordValid = _passwordHasher.Verify(login.Password, user.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Echec de connexion pour {Email}", login.Email);
                throw new InvalidCredentialsException();
            }
            if (!user.IsActive)
            {
                _logger.LogWarning("Compte desactive pour {Email}", login.Email);
                throw new AccountDisabledException();
            }
            _logger.LogInformation("Connexion reussie pour {Email}", login.Email);
            var claims = new Dictionary<string, string>
            {
                [ClaimTypes.Role] = user.Role.ToString()
            };
            var descriptor = new JwtUserDescriptor(user.Id, user.Email, claims);
            string token = _jwtTokenProvider.GenerateToken(descriptor);
            return new LoginResponseDto(token);
        }
    }
}