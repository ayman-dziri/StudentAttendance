using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using StudentAttendance.src.StudentAttendance.Domain.Interfaces.Repositories;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data.Seeders;
using StudentAttendance.src.StudentAttendance.Infrastructure.Interfaces;
using StudentAttendance.src.StudentAttendance.Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Configure MongoDB settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Register MongoDB client factory
builder.Services.AddSingleton<IMongoClientFactory, MongoClientFactory>();

builder.Services.AddFluentValidationAutoValidation();
//validator Services 



// Register repositories
builder.Services.AddScoped<ISessionsRepository, SessionsRepository>();

// Register seeders
builder.Services.AddScoped<SessionsSeeder>();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();


// Seed database
using (var scope = app.Services.CreateScope())
{
    var seedersessions = scope.ServiceProvider.GetRequiredService<SessionsSeeder>();


    await seedersessions.SeedAsync();


}

// Configure the HTTP request pipeline.
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
