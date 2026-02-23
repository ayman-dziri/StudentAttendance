using MongoDB.Bson;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Enums;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Data.Seeders;

public class UsersSeeder
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly ILogger<UsersSeeder> _logger;

    public UsersSeeder(
        IUserRepository userRepository,
        IGroupRepository groupRepository,
        ILogger<UsersSeeder> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SeedAsync()
    {
        try
        {
            var existingUsers = await _userRepository.GetAllUsersAsync();
            if (existingUsers.Any())
            {
                _logger.LogInformation("Users already exist. Seeding skipped.");
                return;
            }

            _logger.LogInformation("Seeding users...");

            // récupérer groupes existants
            var groupG1 = await _groupRepository.GetByNameAsync("G1");
            var groupG2 = await _groupRepository.GetByNameAsync("G2");

            if (groupG1 == null || groupG2 == null)
            {
                _logger.LogWarning("Groups not found. Make sure GroupsSeeder runs first.");
                return;
            }

            var users = GetSeedUsers(groupG1.Id, groupG2.Id);

            foreach (var user in users)
            {
                await _userRepository.CreateUserAsync(user);
                _logger.LogInformation("Seeded user: {UserEmail}", user.Email);
            }

            _logger.LogInformation("Users seeding completed. {Count} users added.", users.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding users");
            throw;
        }
    }

    private List<User> GetSeedUsers(string group1Id, string group2Id)
    {
        return new List<User>
        {
            

            new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@school.com",
                Password = "Password123!", // ⚠️ en vrai : hash obligatoire
                BirthDate = new DateOnly(1985, 5, 10),
                Role = Role.TEACHER,
                IsActive = true,
                GroupId = null
            },

            new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                FirstName = "Anna",
                LastName = "Smith",
                Email = "anna.smith@school.com",
                Password = "Password123!",
                BirthDate = new DateOnly(1990, 8, 15),
                Role = Role.TEACHER,
                IsActive = true,
                GroupId = null
            },

           

            new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                FirstName = "Ali",
                LastName = "Benali",
                Email = "ali.g1@school.com",
                Password = "Student123!",
                BirthDate = new DateOnly(2005, 3, 12),
                Role = Role.STUDENT,
                IsActive = true,
                GroupId = group1Id
            },

            new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                FirstName = "Sara",
                LastName = "Kamal",
                Email = "sara.g1@school.com",
                Password = "Student123!",
                BirthDate = new DateOnly(2006, 7, 5),
                Role = Role.STUDENT,
                IsActive = true,
                GroupId = group1Id
            },

            
                
            new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                FirstName = "Youssef",
                LastName = "Amrani",
                Email = "youssef.g2@school.com",
                Password = "Student123!",
                BirthDate = new DateOnly(2005, 11, 2),
                Role = Role.STUDENT,
                IsActive = true,
                GroupId = group2Id
            },

            new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                FirstName = "Lina",
                LastName = "Zahra",
                Email = "lina.g2@school.com",
                Password = "Student123!",
                BirthDate = new DateOnly(2006, 1, 18),
                Role = Role.STUDENT,
                IsActive = true,
                GroupId = group2Id
            }
        };
    }
}