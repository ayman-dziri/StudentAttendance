using FluentValidation;
using FluentValidation.AspNetCore;
using StudentAttendance.API.Configuration;
using StudentAttendance.src.StudentAttendance.API.Middlewares;
using StudentAttendance.src.StudentAttendance.Application.FluentDTOsValidators;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Application.Services;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data.Seeders;
using StudentAttendance.src.StudentAttendance.Infrastructure.DependencyInjection;
using StudentAttendance.src.StudentAttendance.Infrastructure.Interfaces;
using StudentAttendance.src.StudentAttendance.Infrastructure.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.AddSingleton<IMongoClientFactory, MongoClientFactory>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddValidatorsFromAssemblyContaining<CreateSessionRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateSessionRequestValidator>();

// Seeders
builder.Services.AddScoped<SessionsSeeder>();
builder.Services.AddScoped<UsersSeeder>();
builder.Services.AddScoped<GroupsSeeder>();

// Services Application
builder.Services.AddScoped<ISessionsService, SessionsService>();
builder.Services.AddScoped<ISessionConflictValidator, SessionConflictValidator>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("SwaggerCors", policy =>
        policy.WithOrigins("http://localhost:54812")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var seederusers = scope.ServiceProvider.GetRequiredService<UsersSeeder>();
    var seedergroups = scope.ServiceProvider.GetRequiredService<GroupsSeeder>();
    var seedersessions = scope.ServiceProvider.GetRequiredService<SessionsSeeder>();

    await seedergroups.SeedAsync();
    await seederusers.SeedAsync();
    await seedersessions.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("SwaggerCors");
app.UseAuthorization();
app.MapControllers();

app.Run();