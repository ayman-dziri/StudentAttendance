using Microsoft.Extensions.Options;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Auth;
using StudentAttendance.src.StudentAttendance.Application.Exceptions;
using StudentAttendance.src.StudentAttendance.Application.Interfaces;
using StudentAttendance.src.StudentAttendance.Domain.Auth;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces;
using StudentAttendance.src.StudentAttendance.Infrastructure.Auth;
using StudentAttendance.src.StudentAttendance.Domain.Repositories;

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
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto login, CancellationToken ct = default)
        {
            var user = await _userRepository.GetUserByEmailAsync(login.Email, ct);
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
            var accessExp = now.AddMinutes(_jwtOptions.ExpMinuts);

            // refresh token (création de session)
            var refreshToken = _refreshGen.Generate();
            var refreshExp = now.AddDays(7);

            user.SetRefreshToken(refreshToken, refreshExp);

            var ok = await _userRepository.UpdateRefreshTokenAsync(
                user.Id,
                user.RefreshToken,
                user.RefreshTokenExpiresAt,
                user.RefreshTokenRevokedAt,
                ct);

            if (!ok) throw new Exception("Could not persist refresh token on login.");

            _logger.LogInformation("Connexion reussie pour {Email}", login.Email);

            return new LoginResponseDto(accessToken, refreshToken, accessExp, refreshExp);
        }


        // ---------------- REFRESH TOKEN ----------------
        public async Task<TokenResponse> RefreshAsync(string refreshToken, CancellationToken ct = default)
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
                new Dictionary<string, string> { ["role"] = user.Role.ToString()}
            );

            var accessToken = _jwtTokenProvider.GenerateToken(descriptor);
            var accessExp = now.AddMinutes(_jwtOptions.ExpMinuts);

            // rotation refresh token
            var newRefreshToken = _refreshGen.Generate();
            var refreshExp = now.AddDays(7);

            user.SetRefreshToken(newRefreshToken, refreshExp);

            var ok = await _userRepository.UpdateRefreshTokenAsync(
                user.Id,
                user.RefreshToken,
                user.RefreshTokenExpiresAt,
                user.RefreshTokenRevokedAt,
                ct);

            if (!ok) throw new Exception("Could not update refresh token.");

            return new TokenResponse(accessToken, newRefreshToken, accessExp, refreshExp);
        }

        // ---------------- LOGOUT ----------------
        public async Task LogoutAsync(string userId, CancellationToken ct = default)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, ct);
            if (user is null) return;

            var now = DateTime.UtcNow;

            // Variante A (recommandée) : révoquer + garder le token stocké (pour audit)
            user.RevokeRefreshToken(now);

            // Variante B (plus strict) : supprimer le token complètement
            // user.RefreshToken = null;
            // user.RefreshTokenExpiresAt = null;
            // user.RefreshTokenRevokedAt = now;

            await _userRepository.UpdateRefreshTokenAsync(
                user.Id,
                user.RefreshToken,
                user.RefreshTokenExpiresAt,
                user.RefreshTokenRevokedAt,
                ct
            );
        }
        // ---------------- INVALIDATE ALL  ----------------
        public async Task InvalidateAllAsync(string userId, CancellationToken ct = default)
        {
            await _userRepository.InvalidateAllAsync(userId, ct);
        }
    }
}