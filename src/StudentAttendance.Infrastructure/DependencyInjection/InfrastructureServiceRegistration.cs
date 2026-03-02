using StudentAttendance.src.StudentAttendance.Domain.Auth;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Domain.Repositories;
using StudentAttendance.src.StudentAttendance.Infrastructure.Auth;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
using StudentAttendance.src.StudentAttendance.Infrastructure.Providers;
using StudentAttendance.src.StudentAttendance.Infrastructure.Repositories;

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
        var mongoSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()
            ?? throw new InvalidOperationException("MongoDbSettings section is missing in appsettings.json");

        services.AddSingleton(mongoSettings);
        services.AddSingleton<MongoDbContext>();

        // Repositories
        services.AddScoped<ISessionsRepository, SessionsRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IGroupRepository, GroupRepository>();

        // Auth
        // Token provider
        services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();

        return services;
    }
} 