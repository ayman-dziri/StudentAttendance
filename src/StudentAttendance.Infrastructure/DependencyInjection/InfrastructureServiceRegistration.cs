
﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;


using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Infrastructure.Repositories;


using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
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
        // Configuration MongoDB
        var mongoSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()
            ?? throw new InvalidOperationException("MongoDbSettings section is missing in appsettings.json");

        services.AddSingleton(mongoSettings);
        services.AddSingleton<MongoDbContext>();

        // Repositories
        // Les repositories seront enregistrés ici au fur et à mesure
        services.AddScoped<IAbsenceRepository, AbsenceRepository>();
        services.AddScoped<ISessionsRepository, SessionsRepository>();


        return services;
    }
}