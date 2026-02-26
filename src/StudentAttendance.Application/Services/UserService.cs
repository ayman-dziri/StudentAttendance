using StudentAttendance.src.StudentAttendance.Application.DTOs.user;
using StudentAttendance.src.StudentAttendance.Application.Exceptions;
using StudentAttendance.src.StudentAttendance.Application.Interfaces;
using StudentAttendance.src.StudentAttendance.Application.Mappers;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Enums;
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
            user.GenerateEmail(); // on appel l'email generé pour l'enregistrer dans la DB
            if (string.IsNullOrWhiteSpace(userDto.Password)) throw new ValidationException("Password is required");

            var passwordRequest = userDto.Password;
            user.Password = _passwordHasher.Hash(passwordRequest); // on enregistre le password haché à la place du password entré par l'user

            if (userDto.Role == Role.STUDENT)    user.GroupId = userDto.GroupId; // si l'user est un STUDENT on l'affecte un group
            else user.GroupId = null; // Sinon le groupId reçoit un null

            await _userRepository.AddAsync(user, ct); // enregistrement
        }

        public async Task<UserDetailsResponse?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ValidationException("User id is required"); // verifier si l'id est bien saisie

            var user = await _userRepository.GetUserByIdAsync(id, ct); // appel du repository
            if(user is null)    throw new NotFoundException($"User with id : '{id}' not found"); // lever une excepiton si l'user n'existe pas dans la DB

            var userDetail = UserMapper.ToUserDetail(user); // mapping vers DTO pour envoyer l'user sans password
            return userDetail;
        }

        public async Task<List<UserDetailsResponse>> GetAllUsersAsync(CancellationToken ct = default)
        {
            var users = await _userRepository.GetUsersAsync(ct);
            if (users is null) throw new NotFoundException("users not found");

            return users.Select(UserMapper.ToUserDetail).ToList(); // mapper tous les users en dto puis l'enregistrer dans une liste
        }

        public async Task<bool> UpdateUserAsync(string id, UpdateUserRequest updateUser, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(id))  throw new ValidationException("User id is required.");
            if (updateUser is null) throw new ValidationException("Update data is required");

            var user = UserMapper.ToEntity(updateUser);

            var IsUpdated = await _userRepository.UpdateUserAsync(id, user, ct);
            if (!IsUpdated) throw new NotFoundException($"User with id : '{id}', not found");

            return IsUpdated;
        }

        public async Task<bool> DeleteUserAsync(string id, CancellationToken ct = default)
        {
            var deleted = await _userRepository.DeleteUserAsync(id, ct);
            if (!deleted)   throw new NotFoundException($"User with id '{id}' not found.");

            return deleted;
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken ct = default)
        {
            var user = _userRepository.GetUserByEmailAsync(email, ct); // on recupere l'user par son email
            if (string.IsNullOrWhiteSpace(email)) throw new ValidationException("the field email is required");
            if(user is null)    throw new NotFoundException($" User with email : '{email}' was not found.");

            return await user;
        }

        public async Task<List<User>> GetStudentsByGroupAsync(string groupId, CancellationToken ct = default) // utilisée dans d'autres services
        {
            var students = await _userRepository.GetStudentsByGroupIdAsync(groupId, ct);
            if (students is null) throw new NotFoundException($"students with this groupId '{groupId}' was not found");

            return students;
        }

    }
}
