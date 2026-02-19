using StudentAttendance.src.StudentAttendance.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Injection des couches
builder.Services.AddInfrastructure(builder.Configuration);

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("SwaggerCors");

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
