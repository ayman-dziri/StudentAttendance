using Microsoft.Extensions.Options;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Auth;
using StudentAttendance.src.StudentAttendance.Application.Exceptions;
using StudentAttendance.src.StudentAttendance.Application.Interfaces;
using StudentAttendance.src.StudentAttendance.Domain.Auth;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces;
using StudentAttendance.src.StudentAttendance.Infrastructure.Auth;
using System.Security.Claims;

namespace StudentAttendance.src.StudentAttendance.Application.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly IRefreshTokenGenerator _refreshGen;
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenProvider jwtTokenProvider,
            IRefreshTokenGenerator refreshGen,
            IOptions<JwtOptions> jwtOptions,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenProvider = jwtTokenProvider;
            _refreshGen = refreshGen;
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
        }

        // ---------------- LOGIN ----------------
        public async Task<LoginResponseDto> LoginAsync(
            LoginRequestDto login,
            CancellationToken ct = default)
        {
            var user = await _userRepository.GetUserByEmailAsync(login.Email, ct);
            if (user is null)
            {
                _logger.LogWarning("Echec de connexion pour {Email}", login.Email);
                throw new InvalidCredentialsException();
            }

            if (!_passwordHasher.Verify(login.Password, user.Password))
            {
                _logger.LogWarning("Echec de connexion pour {Email}", login.Email);
                throw new InvalidCredentialsException();
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Compte désactivé pour {Email}", login.Email);
                throw new AccountDisabledException();
            }

            var now = DateTime.UtcNow;

            // access token
            var descriptor = new JwtUserDescriptor(
                user.Id,
                user.Email,
                new Dictionary<string, string>
                {
                    [ClaimTypes.Role] = user.Role.ToString()
                });

            var accessToken = _jwtTokenProvider.GenerateToken(descriptor);

            // refresh token (généré automatiquement)
            var refreshToken = _refreshGen.Generate();
            var refreshExp = now.AddDays(7);

            user.SetRefreshToken(refreshToken, refreshExp);

            await _userRepository.UpdateRefreshTokenAsync(
                user.Id,
                user.RefreshToken,
                user.RefreshTokenExpiresAt,
                user.RefreshTokenRevokedAt,
                ct);

            _logger.LogInformation("Connexion réussie pour {Email}", login.Email);

            return new LoginResponseDto(
                accessToken,
                refreshToken,
                now.AddMinutes(_jwtOptions.ExpMinuts),
                refreshExp
            );
        }

        // ---------------- REFRESH TOKEN ----------------
        public async Task<TokenResponse> RefreshAsync(
            string refreshToken,
            CancellationToken ct = default)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken, ct);
            if (user is null)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            var now = DateTime.UtcNow;

            if (!user.HasValidRefreshToken(refreshToken, now))
                throw new UnauthorizedAccessException("Refresh token expired or revoked.");

            // new access token
            var descriptor = new JwtUserDescriptor(
                user.Id,
                user.Email,
                new Dictionary<string, string>
                {
                    [ClaimTypes.Role] = user.Role.ToString()
                });

            var newAccessToken = _jwtTokenProvider.GenerateToken(descriptor);
            var accessExp = now.AddMinutes(_jwtOptions.ExpMinuts);

            // refresh token rotation
            var newRefreshToken = _refreshGen.Generate();
            var refreshExp = now.AddDays(7);

            user.SetRefreshToken(newRefreshToken, refreshExp);

            await _userRepository.UpdateRefreshTokenAsync(
                user.Id,
                user.RefreshToken,
                user.RefreshTokenExpiresAt,
                user.RefreshTokenRevokedAt,
                ct);

            return new TokenResponse(
                newAccessToken,
                newRefreshToken,
                accessExp,
                refreshExp
            );
        }

        // ---------------- LOGOUT ----------------
        public async Task LogoutAsync(
            string userId,
            CancellationToken ct = default)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, ct);
            if (user is null) return;

            user.RevokeRefreshToken(DateTime.UtcNow);

            await _userRepository.UpdateRefreshTokenAsync(
                user.Id,
                user.RefreshToken,
                user.RefreshTokenExpiresAt,
                user.RefreshTokenRevokedAt,
                ct);
        }
    }
}