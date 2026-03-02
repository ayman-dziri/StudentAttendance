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

var builder = WebApplication.CreateBuilder(args);

// Infrastructure (MongoDB, Repositories)
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
// Config JWT
builder.Services.AddJwtOptions(builder.Configuration);

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

var app = builder.Build();

// Middleware global d'erreurs
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Authentification et autorisation
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
   app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("SwaggerCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();