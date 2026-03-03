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
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenProvider jwtTokenProvider,
            IRefreshTokenGenerator refreshGen,
            IRefreshTokenService refreshTokenService,
            IOptions<JwtOptions> jwtOptions,
            
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenProvider = jwtTokenProvider;
            _refreshGen = refreshGen;
            _refreshTokenService = refreshTokenService ?? throw new ArgumentNullException(nameof(refreshTokenService));
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
        
        // ------------------- Change Password ------------------------
        
        public async Task ChangePasswordAsync(
    string userId,
    ChangePasswordRequest request,
    CancellationToken cancellationToken = default)
{
    ArgumentNullException.ThrowIfNull(request);

    // Récupération utilisateur
    var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
    if (user is null)
    {
        throw new KeyNotFoundException($"User with Id '{userId}' not found.");
    }

    // Vérification ancien mot de passe
    var isOldPasswordValid = _passwordHasher.Verify(request.OldPassword, user.Password);
    if (!isOldPasswordValid)
    {
        throw new UnauthorizedAccessException("Old password is incorrect.");
    }

    // Vérifier que le nouveau mot de passe est différent
    var isSamePassword = _passwordHasher.Verify(request.NewPassword, user.Password);
    if (isSamePassword)
    {
        throw new InvalidOperationException("New password must be different from the old password.");
    }

    // Validation du nouveau mot de passe
    if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
    {
        throw new ArgumentException("New password must be at least 8 characters long.");
    }

    // Hash du nouveau mot de passe via abstraction
    var newHashedPassword = _passwordHasher.Hash(request.NewPassword);

    user.Password = newHashedPassword;

    var updateSuccess = await _userRepository.UpdateUserAsync(user.Id, user, cancellationToken);
    if (!updateSuccess)
    {
        throw new InvalidOperationException("Failed to update user password. Please try again.");
    }

    // Invalidation de toutes les sessions (refresh tokens)
    await _refreshTokenService.InvalidateAllAsync(userId);
}
      
        

    }
}