using MongoDB.Bson;
using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Enums;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;


namespace StudentAttendance.src.StudentAttendance.Infrastructure.Data.Seeders;

    public class AbsencesSeeder

{
        private readonly IAbsenceRepository _absenceRepository;
        private readonly ILogger<AbsencesSeeder> _logger;


    public AbsencesSeeder(IAbsenceRepository absenceRepository, ILogger<AbsencesSeeder> logger)
    {
        _absenceRepository = absenceRepository ?? throw new ArgumentNullException(nameof(absenceRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    }

            public async Task SeedAsync()
    {
                try
             {
            var existingAbsences = await _absenceRepository.GetAllAbsences();
            if (existingAbsences.Count > 0)
            {
                _logger.LogInformation("Absences already exist. Seeding skipped.");
                return;
            }

            _logger.LogInformation("Seeding absences...");


            var absences = GetSeedAbsences();

           
                await _absenceRepository.InsertManyAsync(absences);
                _logger.LogInformation("Seeded absences: {Count} absences added", absences.Count);
            

            _logger.LogInformation("Absences seeding completed. {Count} absences added.", absences.Count);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding absences");
            throw;
        }
    }


    private List<Absence> GetSeedAbsences()
    {
        return new List<Absence>
    {
        new Absence
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Status = StatusPresence.PRESENT,
            StudentId = ObjectId.GenerateNewId().ToString(),
            SessionId = ObjectId.GenerateNewId().ToString(),
            JustificationDate = null
        },
        new Absence
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Status = StatusPresence.ABSENT,
            StudentId = ObjectId.GenerateNewId().ToString(),
            SessionId = ObjectId.GenerateNewId().ToString(),
            JustificationDate = null
        },
        new Absence
        {
           Id = ObjectId.GenerateNewId().ToString(),
            Status = StatusPresence.JUSTIFIED,
            StudentId = ObjectId.GenerateNewId().ToString(),
            SessionId = ObjectId.GenerateNewId().ToString(),
            JustificationDate = DateTime.UtcNow
        },
        new Absence
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Status = StatusPresence.PRESENT,
            StudentId = ObjectId.GenerateNewId().ToString(),
            SessionId = ObjectId.GenerateNewId().ToString(),
            JustificationDate = null
        }
    };
    }



}

