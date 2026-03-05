using FluentValidation;
using FluentValidation.AspNetCore;
using Scalar.AspNetCore;
using StudentAttendance.src.StudentAttendance.API.DependencyInjection;
using StudentAttendance.src.StudentAttendance.API.Middlewares;
using StudentAttendance.src.StudentAttendance.Application.DependencyInjection;
using StudentAttendance.src.StudentAttendance.Application.FluentDTOsValidators;
using StudentAttendance.src.StudentAttendance.Infrastructure.Auth;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
using StudentAttendance.src.StudentAttendance.Infrastructure.DependencyInjection;
using StudentAttendance.src.StudentAttendance.Infrastructure.Interfaces;
using StudentAttendance.src.StudentAttendance.Infrastructure.Repositories;
using System.Text.Json.Serialization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

Log.Logger = new LoggerConfiguration()
    .Enrich.WithProperty("application", "StudentAttendance")
    .WriteTo.Console()
    .WriteTo.GrafanaLoki("http://my-loki:3100")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();



builder.Services.AddOpenTelemetry()
    .WithTracing(t =>
    {
        t.AddAspNetCoreInstrumentation();
        t.SetSampler(new AlwaysOnSampler());

        t.AddZipkinExporter(o => o.Endpoint = new Uri("http://my-tempo:9411/api/v2/spans"));
    })
    .WithMetrics(m =>
    {
        m.AddAspNetCoreInstrumentation();
        m.AddRuntimeInstrumentation();
        m.AddProcessInstrumentation();
        m.AddPrometheusExporter();
    });


// Infrastructure (MongoDB, Repositories)
builder.Services.AddInfrastructure(builder.Configuration);
// Config JWT
//builder.Services.AddJwtOptions(builder.Configuration);

// Application (Services métier)
builder.Services.AddApplication();

// API (JWT, Scalar, CORS)
builder.Services.AddApi(builder.Configuration);

// MongoDB Settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));


builder.Services.AddSingleton<IMongoClientFactory, MongoClientFactory>();

// Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateSessionRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateSessionRequestValidator>();


// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddOpenApi();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StudentAttendance",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
//Refresh token generator
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
if (jwt is null) throw new InvalidOperationException("Jwt section missing.");
if (string.IsNullOrWhiteSpace(jwt.SigningKey)) throw new InvalidOperationException("Jwt:SigningKey missing.");
if (string.IsNullOrWhiteSpace(jwt.Issuer)) throw new InvalidOperationException("Jwt:Issuer missing.");
if (string.IsNullOrWhiteSpace(jwt.Audience)) throw new InvalidOperationException("Jwt:Audience missing.");

Console.WriteLine($"JWT Issuer='{jwt.Issuer}', Audience='{jwt.Audience}', KeyLen={jwt.SigningKey.Length}");




builder.Services.AddCors(options =>
{
    options.AddPolicy("SwaggerCors", policy =>
        policy.WithOrigins("http://localhost:54811")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

//Health Checks ----------> Like Spring Boot Actuator Package 

builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());







var app = builder.Build();

// Middleware global d'erreurs
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Authentification et autorisation
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{

    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("SwaggerCors");



app.MapControllers();


//endpoints

app.MapHealthChecks("/health");
app.MapPrometheusScrapingEndpoint("/metrics");

app.MapGet("/ping", () =>
{
    Log.Information("Ping called - test Loki ingestion");
    return Results.Ok("ok");
});


await app.RunAsync();