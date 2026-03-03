using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace StudentAttendance.src.StudentAttendance.API.DependencyInjection;

/// <summary>
/// Enregistrement centralisé des dépendances de la couche API
/// </summary>
public static class ApiServiceRegistration
{
    /// <summary>
    /// Ajoute les services de la couche API (JWT, Swagger, CORS)
    /// </summary>
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJwt(configuration);
        services.AddApiDocumentation();
        services.AddCorsPolicy();

        return services;
    }

    /// <summary>
    /// Configure l'authentification JWT
    /// </summary>
    private static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
       var jwtKey = configuration["Jwt:Key"] 
          ?? configuration["Jwt:SigningKey"]
          ?? throw new InvalidOperationException("Jwt:Key ou Jwt:SigningKey est manquant dans appsettings.json");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // Vérifie que le token vient bien de "StudentAttendance"
                ValidateIssuer = true,

                // Vérifie que le token est destiné à "StudentAttendance"
                ValidateAudience = true,

                // Vérifie que le token n'est pas expiré
                ValidateLifetime = true,

                // Vérifie que le token a été signé avec notre clé secrète
                ValidateIssuerSigningKey = true,

                // Valeur exacte de l'émetteur attendu
                ValidIssuer = configuration["Jwt:Issuer"],

                // Valeur exacte du destinataire attendu
                ValidAudience = configuration["Jwt:Audience"],

                // Clé secrète pour vérifier la signature du token
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };

            // Gérer les erreurs JWT dans le format JSON de l'API
            options.Events = new JwtBearerEvents
            {
                // Token manquant ou invalide → 401 JSON
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(
                        System.Text.Json.JsonSerializer.Serialize(
                            new { message = "Token manquant ou invalide." }
                        )
                    );
                },

                // Token valide mais rôle insuffisant → 403 JSON
                OnForbidden = async context =>
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(
                        System.Text.Json.JsonSerializer.Serialize(
                            new { message = "Vous n'avez pas les droits pour accéder à cette ressource." }
                        )
                    );
                }
            };
        });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Configure la documentation API (Swagger + OpenApi)
    /// </summary>
    private static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
    /// <summary>
    /// Configure la politique CORS
    /// </summary>
    private static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("SwaggerCors", policy =>
                policy.WithOrigins("http://localhost:54812")
                      .AllowAnyHeader()
                      .AllowAnyMethod());
        });

        return services;
    }
}