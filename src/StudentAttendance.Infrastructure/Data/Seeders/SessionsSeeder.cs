using MongoDB.Bson;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Infrastructure.Interfaces;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.Data.Seeders;

    public class SessionsSeeder
{
    private readonly ISessionsRepository _sessionsRepository;
    private readonly ILogger<SessionsSeeder> _logger;


    public SessionsSeeder(ISessionsRepository sessionsRepository, ILogger<SessionsSeeder> logger)
    {
        _sessionsRepository = sessionsRepository ?? throw new ArgumentNullException(nameof(sessionsRepository)); 
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SeedAsync()
    {
        try
        {
            var existingSessions = await _sessionsRepository.GetAllSessionsAsync();
            if (existingSessions.Count > 0)
            {
                _logger.LogInformation("Sessions already exist. Seeding skipped.");
                return;
            }

            _logger.LogInformation("Seeding sessions...");


            var sessions = GetSeedSessions();

            foreach (var s in sessions)
            {
                await _sessionsRepository.CreateSessionsAsync(s);
                _logger.LogInformation("Seeded sessions: {SessionId}", s.Id);
            }

            _logger.LogInformation("Sessions seeding completed. {Count} sessions added.", sessions.Count);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding sessions");
            throw;
        }
    }


    private List<Session> GetSeedSessions()
    {
        return new List<Session>
    {
        new Session
        {
            Id = ObjectId.GenerateNewId().ToString(),
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
            TeacherId = ObjectId.GenerateNewId().ToString(),
            Group = "G1",
            IsValidated = false
        },

        new Session
        {
            Id = ObjectId.GenerateNewId().ToString(),
            StartTime = DateTime.UtcNow.AddDays(2),
            EndTime = DateTime.UtcNow.AddDays(2).AddHours(3),
            TeacherId = ObjectId.GenerateNewId().ToString(),
            Group = "G2",
            IsValidated = false
        },

        new Session
        {
            Id = ObjectId.GenerateNewId().ToString(),
            StartTime = DateTime.UtcNow.AddDays(3),
            EndTime = DateTime.UtcNow.AddDays(3).AddHours(1),
            TeacherId = ObjectId.GenerateNewId().ToString(),
            Group = "G1",
            IsValidated = false
        }
        ,
         new Session
        {
            Id = "SESSION_ID_1",
            StartTime = DateTime.UtcNow.AddDays(3),
            EndTime = DateTime.UtcNow.AddDays(3).AddHours(1),
            TeacherId = ObjectId.GenerateNewId().ToString(),
            Group = "G1",
            IsValidated = false
        } ,

          new Session
        {
            Id = "SESSION_ID_2",
            StartTime = DateTime.UtcNow.AddDays(3),
            EndTime = DateTime.UtcNow.AddDays(3).AddHours(1),
            TeacherId = ObjectId.GenerateNewId().ToString(),
            Group = "G2",
            IsValidated = false
        }
    };
    }



}
