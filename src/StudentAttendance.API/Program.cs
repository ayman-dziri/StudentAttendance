using StudentAttendance.src.StudentAttendance.Domain.Entities;
using StudentAttendance.src.StudentAttendance.Domain.Enums;
using StudentAttendance.src.StudentAttendance.Infrastructure.Config;
using StudentAttendance.src.StudentAttendance.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure Mongo
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoSetting"));

builder.Services.AddSingleton<StudentAttendanceDbContext>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StudentAttendanceDbContext>();

    // Petit test d'insertion
    await context.Users.InsertOneAsync(new User
    {
        FirstName = "John",
        LastName = "Doe",
        Email = "john.doe@example.com",
        Password = "hashedpassword",
        Role = Role.STUDENT,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    });
}


app.Run();
