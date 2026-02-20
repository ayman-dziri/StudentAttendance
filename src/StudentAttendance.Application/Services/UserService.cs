using StudentAttendance.src.StudentAttendance.Application.DTOs.user;
using StudentAttendance.src.StudentAttendance.Application.Exceptions;
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
            if (userDto is null) throw new ValidationException("User data is required"); // si l'objet userDto entré par l'utilisateur est null on lance une exception

            var user = UserMapper.ToEntity(userDto); // on map le dto entré vers l'entité
            if (string.IsNullOrWhiteSpace(userDto.Password)) throw new ValidationException("Password is required");

            var passwordRequest = userDto.Password;
            user.Password = _passwordHasher.Hash(passwordRequest); // on enregistre le password haché à la place du password entré par l'user

            await _userRepository.AddAsync(user, ct); // enregistrement
        }

        public async Task<User?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ValidationException("User id is required");

            var user = await _userRepository.GetUserByIdAsync(id, ct);
            if(user is null)    throw new NotFoundException($"User with id : '{id}' not found");

            return user;
        }

        public async Task<List<User>> GetAllUsersAsync(CancellationToken ct = default)
            => await _userRepository.GetUsersAsync(ct);

        public async Task<bool> UpdateUserAsync(UpdateUserRequest updateUser, CancellationToken ct = default)
        {
            if (updateUser is null) throw new ValidationException("Update data is required");

            var user = UserMapper.ToEntity(updateUser);

            var updated = await _userRepository.UpdateUserAsync(user, ct);
            if (!updated) throw new Exception("Failed ti update user");

            return updated;
        }

        public async Task<bool> DeleteUserAsync(string id, CancellationToken ct = default)
        {
            var deleted = await _userRepository.DeleteUserAsync(id, ct);
            if (!deleted)   throw new NotFoundException($"User with id '{id}' not found.");

            return deleted;
        }
    }
}
