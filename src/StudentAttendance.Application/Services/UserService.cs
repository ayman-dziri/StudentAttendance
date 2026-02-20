using StudentAttendance.src.StudentAttendance.Application.DTOs.user;
using StudentAttendance.src.StudentAttendance.Application.Interfaces;
using StudentAttendance.src.StudentAttendance.Application.Mappers;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces;
using StudentAttendance.src.StudentAttendance.Domain.Repositories;

namespace StudentAttendance.src.StudentAttendance.Application.Services
{
    public class UserService : IUserService
    {

        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;

        public UserService(IPasswordHasher passwordHasher, IUserRepository userRepository)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
        }

        public async Task CreateUserAsync(CreateUserRequest userDto, CancellationToken ct = default)
        {
            var user = UserMapper.ToEntity(userDto);

            var passwordRequest = userDto.Password;
            user.Password = _passwordHasher.Hash(passwordRequest);

            await _userRepository.AddAsync(user, ct);
        }

        public async Task<User?> GetByIdAsync(string id, CancellationToken ct = default)
            => await _userRepository.GetUserByIdAsync(id, ct);

        public async Task<List<User>> GetAllUsersAsync(CancellationToken ct = default)
            => await _userRepository.GetUsersAsync(ct);

        public async Task<bool> UpdateUserAsync(UpdateUserRequest updateUser, CancellationToken ct = default)
        {
            var user = UserMapper.ToEntity(updateUser);

            return await _userRepository.UpdateUserAsync(user, ct);
        }

        public async Task<bool> DeleteUserAsync(string id, CancellationToken ct = default)
            => await _userRepository.DeleteUserAsync(id, ct);
    }
}
