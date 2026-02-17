using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentAttendance.src.StudentAttendance.Infrastructure.Configuration;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;

namespace StudentAttendance.src.StudentAttendance.Infrastructure.DependencyInjection;

/// <summary>
/// Enregistrement centralisé des dépendances de la couche Infrastructure
/// </summary>
public static class InfrastructureServiceRegistration
{
    /// <summary>
    /// Ajoute les services d'infrastructure (MongoDB, Repositories) au conteneur DI
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration MongoDB
        var mongoSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()
            ?? throw new InvalidOperationException("MongoDbSettings section is missing in appsettings.json");

        services.AddSingleton(mongoSettings);
        services.AddSingleton<StudentAttendanceDbContext>();

        // Repositories
        // Les repositories seront enregistrés ici au fur et à mesure

        return services;
    }
}