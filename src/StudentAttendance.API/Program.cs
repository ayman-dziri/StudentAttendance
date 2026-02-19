<<<<<<< HEAD
<<<<<<< HEAD

using FluentValidation;
using FluentValidation.AspNetCore;
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
=======
using StudentAttendance.src.StudentAttendance.Infrastructure.DependencyInjection;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Application.Services;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Infrastructure.Repositories;
using StudentAttendance.src.StudentAttendance.API.Middlewares;
>>>>>>> origin/feature/scrum-19-conflit-horaire
=======
﻿using StudentAttendance.src.StudentAttendance.Infrastructure.DependencyInjection;
>>>>>>> origin/feature/scrum-12-attendance-validation

var builder = WebApplication.CreateBuilder(args);

// Injection des couches
builder.Services.AddInfrastructure(builder.Configuration);


// Configure MongoDB settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));


builder.Services.AddSingleton<IMongoClientFactory, MongoClientFactory>();


builder.Services.AddFluentValidationAutoValidation();


//validator Services 

builder.Services.AddValidatorsFromAssemblyContaining<CreateSessionRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateSessionRequestValidator>();





// Register seeders
builder.Services.AddScoped<SessionsSeeder>();
builder.Services.AddScoped<AbsencesSeeder>();


// Services Application
builder.Services.AddScoped<IAbsenceService, AbsenceService>();
builder.Services.AddScoped<ISessionsService, SessionsService>();


// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
<<<<<<< HEAD
=======

// Application service (nom complet)
builder.Services.AddScoped<
    StudentAttendance.src.StudentAttendance.Application.Interfaces.IAttendanceService,
    StudentAttendance.src.StudentAttendance.Application.Interfaces.AttendanceService>();


var useMocks = builder.Configuration.GetValue<bool>("UseMocks");

if (useMocks)
{
    builder.Services.AddSingleton<
        StudentAttendance.src.StudentAttendance.Domain.IRepositories.IAbsenceRepository,
        StudentAttendance.src.StudentAttendance.Infrastructure.Repositories.Mocks.FakeAbsenceRepository>();

    builder.Services.AddSingleton<
        StudentAttendance.src.StudentAttendance.Domain.IRepositories.ISessionRepository,
        StudentAttendance.src.StudentAttendance.Infrastructure.Repositories.Mocks.FakeSessionRepository>();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("SwaggerCors", policy =>
        policy.WithOrigins("http://localhost:54812")
              .AllowAnyHeader()
              .AllowAnyMethod());
});


>>>>>>> origin/feature/scrum-12-attendance-validation
var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();




// Seed database
using (var scope = app.Services.CreateScope())
{
    var seedersessions = scope.ServiceProvider.GetRequiredService<SessionsSeeder>();
    var seederabsences = scope.ServiceProvider.GetRequiredService<AbsencesSeeder>();


    await seedersessions.SeedAsync();
    await seederabsences.SeedAsync();



}

// Middleware global d'erreurs (doit être avant tout le reste)
app.UseMiddleware<ExceptionHandlingMiddleware>();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("SwaggerCors");

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
