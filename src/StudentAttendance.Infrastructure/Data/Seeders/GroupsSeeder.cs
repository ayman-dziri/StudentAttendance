using MongoDB.Bson;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Data.Seeders;

public class GroupsSeeder
{
    private readonly IGroupRepository _groupRepository;
    private readonly ILogger<GroupsSeeder> _logger;

    public GroupsSeeder(IGroupRepository groupRepository, ILogger<GroupsSeeder> logger)
    {
        _groupRepository = groupRepository;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        var existingG1 = await _groupRepository.GetByNameAsync("G1");
        if (existingG1 != null)
        {
            _logger.LogInformation("Groups already seeded.");
            return;
        }

        var groups = new List<Group>
        {
            new Group
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Label = "G1",
                CreatedAt = DateTime.UtcNow
            },
            new Group
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Label = "G2",
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var group in groups)
        {
            await _groupRepository.CreateAsync(group); // assure-toi d'avoir cette méthode
        }

        _logger.LogInformation("Groups seeded successfully.");
    }
}