using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces;
using StudentAttendanceV2.src.StudentAttendance.Application.DTOs.Auth.Requests;
using StudentAttendanceV2.src.StudentAttendance.Application.Interfaces;

namespace StudentAttendanceV2.src.StudentAttendance.Application.Services
{
    public class AuthService : IAuthService
    {

        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenService _refreshTokenService;



        public AuthService(
            IUserRepository userRepository ,
            IRefreshTokenService refreshTokenService
        )
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _refreshTokenService = refreshTokenService ?? throw new ArgumentNullException(nameof(refreshTokenService));
        }






        public async Task ChangePasswordAsync(string userId , ChangePasswordRequest request , CancellationToken cancellationToken = default)
        {

            ArgumentNullException.ThrowIfNull(request);
            //recuprer lutilisateur
            var user = await _userRepository.GetUserByIdAsync(userId , cancellationToken);
            if(user is null)
            {
                throw new KeyNotFoundException($"User with Id '{userId}' not found . ");
            }

            //verifier si lancien mot de passe est incorrect
            bool isValid = BCrypt.Net.BCrypt.Verify(request.OldPassword , user.Password);

            if(!isValid)
            {
                throw new UnauthorizedAccessException("Old Password incorrect");
            }

            //verifier si le nouveau mot de passe est comme le current password
            bool samePassword = BCrypt.Net.BCrypt.Verify(request.NewPassword , user.Password);

            if(samePassword)
            {
                throw new Exception("New Password must be different than old one");
            }

            //validation nouveau mot de passe
            if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
            {
                throw new ArgumentException("New password must be at least 8 characters long.");
            }

            //hasher le mt de passe
            string newhashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            user.Password = newhashedPassword;

            bool updateSuccess = await _userRepository.UpdateUserAsync(user.Id, user, cancellationToken);
            if(!updateSuccess)
            {
                throw new InvalidOperationException("Failed to update user password. Please try again");
            } 
            //// Invalide tous les refresh tokens (sécurité post-changement MDP)
            await _refreshTokenService.InvalidateAllAsync(userId);

        }
    }
}