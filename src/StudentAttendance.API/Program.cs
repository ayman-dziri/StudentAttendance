

using FluentValidation;
using FluentValidation.AspNetCore;
using StudentAttendance.API.Configuration;
using StudentAttendance.src.StudentAttendance.API.Middlewares;
using StudentAttendance.src.StudentAttendance.API.Middlewares;
using StudentAttendance.src.StudentAttendance.Application.FluentDTOsValidators;
using StudentAttendance.src.StudentAttendance.Application.Interfaces.Services;
using StudentAttendance.src.StudentAttendance.Application.Services;
using StudentAttendance.src.StudentAttendance.Application.Services;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;

using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data.Seeders;
using StudentAttendance.src.StudentAttendance.Infrastructure.DependencyInjection;
using StudentAttendance.src.StudentAttendance.Infrastructure.DependencyInjection;
using StudentAttendance.src.StudentAttendance.Infrastructure.DependencyInjection;
using StudentAttendance.src.StudentAttendance.Infrastructure.Interfaces;
using StudentAttendance.src.StudentAttendance.Infrastructure.Repositories;
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


//validator Services 

builder.Services.AddValidatorsFromAssemblyContaining<CreateSessionRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateSessionRequestValidator>();





// Register seeders
builder.Services.AddScoped<SessionsSeeder>();
builder.Services.AddScoped<AbsencesSeeder>();
builder.Services.AddScoped<UsersSeeder>();
builder.Services.AddScoped<GroupsSeeder>();


// Services Application
builder.Services.AddScoped<IAbsenceService, AbsenceService>();
builder.Services.AddScoped<ISessionsService, SessionsService>();

builder.Services.AddScoped<ISessionConflictValidator, SessionConflictValidator>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();



// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();


// Application service (nom complet)
builder.Services.AddScoped<
    IAttendanceService,
    AttendanceService>();


//var useMocks = builder.Configuration.GetValue<bool>("UseMocks");

//if (useMocks)
//{
//    builder.Services.AddSingleton<
//        StudentAttendance.src.StudentAttendance.Domain.IRepositories.IAbsenceRepository,
//        StudentAttendance.src.StudentAttendance.Infrastructure.Repositories.Mocks.FakeAbsenceRepository>();

//    builder.Services.AddSingleton<
//        StudentAttendance.src.StudentAttendance.Domain.IRepositories.ISessionRepository,
//        StudentAttendance.src.StudentAttendance.Infrastructure.Repositories.Mocks.FakeSessionRepository>();
//}

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
    var seedersessions = scope.ServiceProvider.GetRequiredService<SessionsSeeder>();
    var seederabsences = scope.ServiceProvider.GetRequiredService<AbsencesSeeder>();
    var seederusers = scope.ServiceProvider.GetRequiredService<UsersSeeder>();
    var seedergroups = scope.ServiceProvider.GetRequiredService<GroupsSeeder>();



    await seedersessions.SeedAsync();
    await seederabsences.SeedAsync();
    await seederusers.SeedAsync();
    await seedergroups.SeedAsync();




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
