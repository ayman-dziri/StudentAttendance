
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

var app = builder.Build();




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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();