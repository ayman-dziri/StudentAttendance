namespace StudentAttendance.src.StudentAttendance.Infrastructure.Configuration;

/// <summary>
/// Configuration de connexion à MongoDB lue depuis appsettings.json
/// </summary>
public sealed class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}